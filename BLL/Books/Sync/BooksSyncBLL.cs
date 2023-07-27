using BookshelfModels.Books;
using BookshelfServices.Books.Api;
using BookshelfServices.User;
using Plugin.Connectivity;

namespace BookshelfServices.Books.Sync
{
    public class BooksSyncBLL : IBooksSyncBLL
    {
        public static SyncStatus Synchronizing { get; set; }

        public enum SyncStatus
        {
            Processing, Sleeping, ServerOff
        }

       public Timer? Timer { get; set; }
        readonly int Interval = 60000;
       public bool ThreadIsRunning { get; set; } = false;


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

                Timer = new Timer(SyncLocalDb, null, Interval, Timeout.Infinite);
            }
        }

        private async void SyncLocalDb(object? state)
        {
            try
            {
                Models.User? user = await UserBLL.GetUserLocal();

                if (user != null && Synchronizing != SyncStatus.Processing)
                {
                    Synchronizing = SyncStatus.Processing;

                    if (CrossConnectivity.Current.IsConnected)
                    {
                        DateTime LastUpdate = user.LastUpdate;

                        List<Book> booksList = await BookshelfRepos.Books.BooksLocalDAL.GetBooksByLastUpdate(user.Id, user.LastUpdate);

                        //update api database
                        foreach (Book book in booksList)
                        {
                            //if the book has a local temporary Guid key, register it in the firebase
                            if (book.LocalTempId != null)
                            {
                                //define the key has a null for register the book in firebase
                                var addBookResp = await BooksApiBLL.AddBook(book);

                                if (addBookResp.Success && addBookResp.Content is not null)
                                {
                                    string localTempId = book.LocalTempId;
                                    book.LocalTempId = null;
                                    await BookshelfRepos.Books.BooksLocalDAL.UpdateBookId(localTempId, Convert.ToString(addBookResp.Content), user.Id);
                                }
                                else throw new Exception($"Não foi possivel sincronizar o livro {book.Id}, res: {addBookResp.Error}");
                            }
                            else
                                await BookshelfRepos.Books.BooksLocalDAL.UpdateBook(book, user.Id);
                        }

                        var respGetBooksByLastUpdate = await BooksApiBLL.GetBooksByLastUpdate(user.LastUpdate);

                        if (respGetBooksByLastUpdate.Success && respGetBooksByLastUpdate.Content is not null)
                        {
                            List<Book>? BooksByLastUpdate = respGetBooksByLastUpdate.Content as List<Book>;

                            if (BooksByLastUpdate is not null)
                            {
                                foreach (Book book in BooksByLastUpdate)
                                {
                                    await BookshelfRepos.Books.BooksLocalDAL.AddOrUpdateBook(book, user.Id);

                                    if (LastUpdate < book.UpdatedAt) LastUpdate = book.UpdatedAt;
                                }
                            }
                        }

                        await BookshelfRepos.User.UserLocalDAL.UpdateUserLastUpdateLocal(user.Id, LastUpdate);
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
                Timer?.Change(Interval, Timeout.Infinite);
            }
        }
    }
}
