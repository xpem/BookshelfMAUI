using BookshelfModels.Books;
using BookshelfServices.Books.Api;
using Plugin.Connectivity;

namespace BookshelfServices.Books
{
    public class BooksServices : IBooksServices
    {
        public BookshelfModels.User.User? User { get; set; }
        readonly IBooksApiServices booksApiServices;

        public BooksServices(IBooksApiServices _booksApiServices)
        {
            User = BookshelfRepos.User.UserRepos.GetUser();
            booksApiServices = _booksApiServices;
        }

        public async Task<Totals> GetBookshelfTotals()
        {
            Totals BTotals = new();

            if (User?.Id != null)
            {
                List<(Situation, int)> list = await BookshelfRepos.Books.BooksRepos.GetBookshelfTotals(User.Id);

                if (list.Count > 0)
                {
                    BTotals.IllRead = list.Where(a => a.Item1 == Situation.IllRead).FirstOrDefault().Item2;
                    BTotals.Reading = list.Where(a => a.Item1 == Situation.Reading).FirstOrDefault().Item2;
                    BTotals.Read = list.Where(a => a.Item1 == Situation.Read).FirstOrDefault().Item2;
                    BTotals.Interrupted = list.Where(a => a.Item1 == Situation.Interrupted).FirstOrDefault().Item2;
                }
                else
                {
                    BTotals.IllRead = BTotals.Reading = BTotals.Read = BTotals.Interrupted = 0;
                }
            }

            return BTotals;
        }

        public async Task<Book?> GetBook(string bookKey)
        {
            if (User?.Id != null)
            {
                return await BookshelfRepos.Books.BooksRepos.GetBook(User.Id, bookKey);
            }

            return null;
        }

        public async Task<string?> UpdateBook(Book book)
        {
            if (User?.Id != null)
            {
                book.LastUpdate = DateTime.Now;

                BookshelfRepos.Books.BooksRepos.UpdateBook(book, User.Id);
                //
                if (CrossConnectivity.Current.IsConnected)
                {
                    (bool res, string message) = await booksApiServices.UpdateBook(book, User);

                    if (!res) { return message; }
                }
            }
            return null;
        }

        public async Task<string?> AddBook(Book book)
        {
            book.LastUpdate = DateTime.Now;
            if (User?.Id != null)
            {
                if (CrossConnectivity.Current.IsConnected)
                {
                    (bool success, string res) = await booksApiServices.AddBook(book, User);

                    if (success) { book.BookKey = res; }
                    else return res;

                }
                else
                {
                    //temporary local UID
                    book.BookKey = Guid.NewGuid().ToString();
                }

                BookshelfRepos.Books.BooksRepos.AddBook(book, User.Id);
            }

            return null;
        }

        public async Task<bool> VerifyBookbyTitle(string title)
        {
            bool ret = false;

            if (User?.Id != null)
                ret = await BookshelfRepos.Books.BooksRepos.GetBookByTitle(User.Id, title);

            return ret;

        }
    }
}
