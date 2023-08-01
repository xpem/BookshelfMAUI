using BLL.Books;
using BLL.Books.Historic;
using BLL.User;
using LocalDbDAL.Books.BookHistoric;
using Models.Books;
using Models.Books.Historic;
using Plugin.Connectivity;

namespace BLL.Sync
{
    public class BooksSyncBLL : IBooksSyncBLL
    {
        public static SyncStatus Synchronizing { get; set; }

        public enum SyncStatus
        {
            Processing, Sleeping, ServerOff
        }

        private readonly IBookHistoricLocalDAL BookHistoricLocalDAL;
        readonly IBookHistoricApiBLL BookHistoricApiBLL;

        public BooksSyncBLL(IBookHistoricLocalDAL bookHistoricLocalDAL, IBookHistoricApiBLL bookHistoricApiBLL)
        {
            BookHistoricLocalDAL = bookHistoricLocalDAL;
            BookHistoricApiBLL = bookHistoricApiBLL;
        }

        public Timer? Timer { get; set; }

        //30 secs
        readonly int Interval = 30000;

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

                        List<Book> booksList = await LocalDbDAL.Books.BooksLocalDAL.GetBooksByLastUpdate(user.Id, user.LastUpdate);

                        //update api database
                        foreach (Book book in booksList)
                        {
                            //if the book has a local temporary Guid key, register it in the firebase
                            if (book.LocalTempId != null)
                            {
                                //define the key has a null for register the book in firebase
                                Models.Responses.BLLResponse addBookResp = await BooksApiBLL.AddBook(book);

                                if (addBookResp.Success && addBookResp.Content is not null)
                                {
                                    string localTempId = book.LocalTempId;
                                    book.LocalTempId = null;
                                    await LocalDbDAL.Books.BooksLocalDAL.UpdateBookId(localTempId, Convert.ToString(addBookResp.Content), user.Id);
                                }
                                else throw new Exception($"Não foi possivel sincronizar o livro {book.Id}, res: {addBookResp.Error}");
                            }
                            else
                                await LocalDbDAL.Books.BooksLocalDAL.UpdateBook(book, user.Id);
                        }

                        //update local database
                        Models.Responses.BLLResponse respGetBooksByLastUpdate = await BooksApiBLL.GetBooksByLastUpdate(user.LastUpdate);

                        if (respGetBooksByLastUpdate.Success && respGetBooksByLastUpdate.Content is not null)
                        {
                            List<Book>? BooksByLastUpdate = respGetBooksByLastUpdate.Content as List<Book>;

                            if (BooksByLastUpdate is not null)
                            {
                                foreach (Book book in BooksByLastUpdate)
                                {
                                    await LocalDbDAL.Books.BooksLocalDAL.AddOrUpdateBook(book, user.Id);

                                    if (LastUpdate < book.UpdatedAt) LastUpdate = book.UpdatedAt;
                                }
                            }
                        }

                        //  var respGetBookHistoricListByCreatedAt = await BookHistoricApiBLL.GetBookHistoricByLastCreatedAt(user.LastUpdate);

                        Models.Responses.BLLResponse respGetBookHistoricListByCreatedAt = await BookHistoricApiBLL.GetBookHistoricByLastCreatedAt(DateTime.MinValue);

                        if (respGetBookHistoricListByCreatedAt.Success && respGetBookHistoricListByCreatedAt.Content is not null)
                        {
                            List<BookHistoric>? bookHistoricsList = respGetBookHistoricListByCreatedAt.Content as List<BookHistoric>;

                            if (bookHistoricsList is not null)
                                foreach (BookHistoric bookHistoric in bookHistoricsList)
                                {
                                    if (!await BookHistoricLocalDAL.CheckBookHistoricById(bookHistoric.Id))
                                        await BookHistoricLocalDAL.AddBookHistoric(bookHistoric, user.Id);
                                }
                        }

                        await LocalDbDAL.User.UserLocalDAL.UpdateUserLastUpdateLocal(user.Id, LastUpdate);
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
