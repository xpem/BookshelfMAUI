using BookshelfModels.Books;
using BLL.Books.Api;
using BLL.User;
using Plugin.Connectivity;

namespace BLL.Books.Sync
{
    public class BooksSyncServices : IBooksSyncServices
    {
        private readonly IUserServices userServices;
        private readonly IBooksApiServices booksApiServices;

        public BooksSyncServices(IUserServices _userServices, IBooksApiServices _booksApiServices)
        {
            userServices = _userServices;
            booksApiServices = _booksApiServices;
        }

        public static SyncStatus Synchronizing { get; set; }

        public enum SyncStatus
        {
            Processing, Sleeping, ServerOff
        }

        public static Timer? _Timer;
        readonly int Interval = 60000;
        public static bool ThreadIsRunning = false;


        public void StartThread()
        {
            if (!ThreadIsRunning)
            {
                Synchronizing = SyncStatus.Sleeping;

                Thread thread = new(SetTimer) { IsBackground = true };
                thread.Start();
            }
        }

        public void SetTimer()
        {
            if (!ThreadIsRunning)
            {
                ThreadIsRunning = true;
                SyncLocalDb(null);

                _Timer = new Timer(SyncLocalDb, null, Interval, Timeout.Infinite);
            }
        }

        private async void SyncLocalDb(object? state)
        {
            try
            {
                BookshelfModels.User.User? user = await userServices.GetUserLocal();

                if (user != null && Synchronizing != SyncStatus.Processing)
                {
                    Synchronizing = SyncStatus.Processing;

                    if (CrossConnectivity.Current.IsConnected)
                    {
                        DateTime LastUpdate = user.LastUpdate;

                        List<Book> booksList = await BookshelfRepos.Books.BooksRepos.GetBooksByLastUpdate(user.Id, user.LastUpdate);

                        //update api database
                        foreach (Book book in booksList)
                        {
                            //if the book has a local temporary Guid key, register it in the firebase
                            if (book.LocalTempId != null)
                            {
                                //define the key has a null for register the book in firebase
                                (bool success, string? res) = await booksApiServices.AddBook(book, user);

                                if (success && !string.IsNullOrEmpty(res))
                                {
                                    string localTempId = book.LocalTempId;
                                    book.LocalTempId = null;
                                    await BookshelfRepos.Books.BooksRepos.UpdateBookId(localTempId, res, user.Id);
                                }
                                else throw new Exception($"Não foi possivel sincronizar o livro {book.Id}, res: {res}");
                            }
                            else
                                await BookshelfRepos.Books.BooksRepos.UpdateBook(book, user.Id);
                        }

                        List<Book>? BooksByLastUpdate = await booksApiServices.GetBooksByLastUpdate(user);

                        if (BooksByLastUpdate is not null)
                        {
                            foreach (Book book in BooksByLastUpdate)
                            {
                                await BookshelfRepos.Books.BooksRepos.AddOrUpdateBook(book, user.Id);

                                if (LastUpdate < book.UpdatedAt) LastUpdate = book.UpdatedAt;
                            }
                        }

                        await BookshelfRepos.User.UserRepos.UpdateUserLastUpdateLocal(user.Id, LastUpdate);
                    }

                    Synchronizing = SyncStatus.Sleeping;
                }
            }
            catch (HttpRequestException ex)
            {
                if (ex.InnerException != null && ex.InnerException.Message.Contains("No connection could be made because the target machine actively refused it."))
                { Synchronizing = SyncStatus.ServerOff; }
                else throw ex;
            }
            catch { throw; }
            finally
            {
                _Timer?.Change(Interval, Timeout.Infinite);
            }
        }
    }
}
