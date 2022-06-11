using BookshelfModels.Books;
using BookshelfRepos.User;
using BookshelfServices.Books.Api;
using Plugin.Connectivity;

namespace BookshelfServices.Books
{
    public class BooksServices : IBooksServices
    {
        readonly IBooksApiServices booksApiServices;

        public BooksServices(IBooksApiServices _booksApiServices)
        {
            booksApiServices = _booksApiServices;
        }

        public async Task<Totals> GetBookshelfTotals()
        {
            Totals BTotals = new();

            BookshelfModels.User.User? User  = BookshelfRepos.User.UserRepos.GetUser();
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
            BookshelfModels.User.User? User = BookshelfRepos.User.UserRepos.GetUser();
            if (User?.Id != null)
            {
                return await BookshelfRepos.Books.BooksRepos.GetBook(User.Id, bookKey);
            }

            return null;
        }

        public async Task<string?> UpdateBook(Book book)
        {
            BookshelfModels.User.User? User = BookshelfRepos.User.UserRepos.GetUser();
            if (User?.Id != null)
            {
                book.LastUpdate = DateTime.Now;

                BookshelfRepos.Books.BooksRepos.UpdateBook(book, User.Id);
                //
                if (CrossConnectivity.Current.IsConnected)
                {
                    (bool res, string? message) = await booksApiServices.UpdateBook(book, User);

                    if (!res) { return message; }
                }
            }
            return null;
        }

        public async Task<string?> AddBook(Book book)
        {
            book.LastUpdate = DateTime.Now;

            BookshelfModels.User.User? User = BookshelfRepos.User.UserRepos.GetUser();
            if (User?.Id != null)
            {
                if (CrossConnectivity.Current.IsConnected)
                {
                    (bool success, string? res) = await booksApiServices.AddBook(book, User);

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

            BookshelfModels.User.User? User = BookshelfRepos.User.UserRepos.GetUser();
            if (User?.Id != null)
                ret = await BookshelfRepos.Books.BooksRepos.GetBookByTitle(User.Id, title);

            return ret;

        }

        /// <summary>
        /// Get books situations by status
        /// </summary>
        /// <param name="Situation"></param>
        /// <returns></returns>
        public async Task<(List<UIBookItem>, int total)> GetBookSituationByStatus(int page, int Situation, string? textoBusca = null)
        {
            List<UIBookItem> listBooksItens = new();
            int total = 0;

            BookshelfModels.User.User? User = BookshelfRepos.User.UserRepos.GetUser();
            if (User?.Id != null)
            {
                int pageSize = 10;

                List<Book> list = (await BookshelfRepos.Books.BooksRepos.GetBookSituationByStatus(Situation, User.Id, textoBusca));

                total = list.Count;

                list = list.Skip((page - 1) * pageSize).Take(pageSize).ToList();


                string SubtitleAndVol;

                foreach (Book book in list)
                {
                    SubtitleAndVol = "";
                    if (!string.IsNullOrEmpty(book.SubTitle))
                    {
                        SubtitleAndVol = book.SubTitle;
                    }
                    if (!string.IsNullOrEmpty(book.SubTitle) && !string.IsNullOrEmpty(book.Volume))
                    {
                        SubtitleAndVol += "; ";
                    }
                    if (!string.IsNullOrEmpty(book.Volume))
                    {
                        SubtitleAndVol += "Vol.: " + book.Volume;
                    }

                    UIBookItem bookItem = new()
                    {
                        Key = book.BookKey,
                        Title = book.Title,
                        AuthorsAndYear = book.Authors + "; Ano: " + book.Year,
                        Pages = book.Pages.ToString(),
                        SubtitleAndVol = SubtitleAndVol,
                    };

                    if ((Situation)Situation == BookshelfModels.Books.Situation.Read)
                    {
                        bookItem.Rate = book.Rating?.Rate > 0 ? string.Format("Avaliação pessoal: {0} de 5", book.Rating.Rate.ToString()) : "";
                    }

                    listBooksItens.Add(bookItem);
                }
            }

            return (listBooksItens, total);
        }

        public async void InactivateBook(string bookKey)
        {
            Book? book = await GetBook(bookKey);

            BookshelfModels.User.User? User = BookshelfRepos.User.UserRepos.GetUser();
            if (book?.BookKey is not null && User?.Id is not null)
            {
                book.LastUpdate = DateTime.Now;
                book.Inactive = true;

                BookshelfRepos.Books.BooksRepos.InactivateBook(book.BookKey, User.Id, book.LastUpdate);

                if (CrossConnectivity.Current.IsConnected)
                {
                    await booksApiServices.UpdateBook(book, User);
                }
            }
        }

        public async void UpdateBookSituation(string Key, Situation situation, int rate, string comment)
        {
            Book? book = await GetBook(Key);

            BookshelfModels.User.User? User = UserRepos.GetUser();
            if (book is not null && User?.Id is not null)
            {
                book.LastUpdate = DateTime.Now;
                book.Situation = situation;

                if (book.Rating is null)
                {
                    book.Rating = new();
                }

                book.Rating.Rate = rate;
                book.Rating.Comment = comment;

                BookshelfRepos.Books.BooksRepos.UpdateBook(book, User.Id);

                if (CrossConnectivity.Current.IsConnected)
                {
                    _ = booksApiServices.UpdateBook(book, User);
                }
            }
        }
    }
}
