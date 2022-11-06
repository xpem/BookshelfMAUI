using BookshelfModels.Books;
using BookshelfServices.Books.Api;
using BookshelfServices.User;
using Plugin.Connectivity;

namespace BookshelfServices.Books.Sync
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

        public static bool Synchronizing { get; set; }

        public static bool ThreadIsRunning { get; set; }


        public void StartThread()
        {
            if (!ThreadIsRunning)
            {
                ThreadIsRunning = true;
                Thread thread = new(SyncLocalDb) { IsBackground = true };
                thread.Start();
            }
        }

        public async void ContinuosSync()
        {
            try
            {
                while (ThreadIsRunning)
                {
                    SyncLocalDb();

                    //delay of three minutes 
                    await Task.Delay(180000);
                }
            }
            catch (Exception ex) { throw ex; }
        }

        public async void SyncLocalDb()
        {
            try
            {
                BookshelfModels.User.User? user = userServices.GetUserLocal();

                if (user != null && !Synchronizing)
                {
                    Synchronizing = true;

                    if (CrossConnectivity.Current.IsConnected)
                    {
                        DateTime LastUptade = user.LastUpdate;

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
                                    BookshelfRepos.Books.BooksRepos.UpdateBookId(localTempId, res, user.Id);
                                }
                                else throw new Exception($"Não foi possivel sincronizar o livro {book.Id}, res: {res}");
                            }
                            else
                                BookshelfRepos.Books.BooksRepos.UpdateBook(book, user.Id);
                        }

                        List<Book>? BooksByLastUpdate = await booksApiServices.GetBooksByLastUpdate(user);

                        if (BooksByLastUpdate is not null)
                        {
                            foreach (Book book in BooksByLastUpdate)
                            {
                                BookshelfRepos.Books.BooksRepos.AddOrUpdateBook(book, user.Id);

                                if (LastUptade < book.UpdatedAt) LastUptade = book.UpdatedAt;
                            }
                        }

                        BookshelfRepos.User.UserRepos.UpdateUserLastUpdateLocal(user.Id, LastUptade);
                    }
                    Synchronizing = false;
                }
            }
            catch (Exception ex) { throw ex; }
        }
    }
}
