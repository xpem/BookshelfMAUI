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

            BookshelfModels.User.User? User = await UserRepos.GetUser();
            if (User?.Id != null)
            {
                List<(Status, int)> list = await BookshelfRepos.Books.BooksRepos.GetBookshelfTotals(User.Id);

                if (list.Count > 0)
                {
                    BTotals.IllRead = list.Where(a => a.Item1 == Status.IllRead).FirstOrDefault().Item2;
                    BTotals.Reading = list.Where(a => a.Item1 == Status.Reading).FirstOrDefault().Item2;
                    BTotals.Read = list.Where(a => a.Item1 == Status.Read).FirstOrDefault().Item2;
                    BTotals.Interrupted = list.Where(a => a.Item1 == Status.Interrupted).FirstOrDefault().Item2;
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
            BookshelfModels.User.User? User = await UserRepos.GetUser();
            if (User?.Id != null)
                return await BookshelfRepos.Books.BooksRepos.GetBook(User.Id, bookKey);

            return null;
        }

        public async Task<string?> UpdateBook(Book book)
        {
            BookshelfModels.User.User? User = await UserRepos.GetUser();
            if (User?.Id != null)
            {
                book.UpdatedAt = DateTime.Now;

                await BookshelfRepos.Books.BooksRepos.UpdateBook(book, User.Id);
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
            book.UpdatedAt = DateTime.Now;

            BookshelfModels.User.User? User = await UserRepos.GetUser();

            if (User?.Id != null)
            {
                if (CrossConnectivity.Current.IsConnected)
                {
                    (bool success, string? res) = await booksApiServices.AddBook(book, User);

                    if (success) { book.Id = Convert.ToInt32(res); }
                    else return res;

                }
                else
                {
                    book.LocalTempId = Guid.NewGuid().ToString();
                }

                await BookshelfRepos.Books.BooksRepos.AddBook(book, User.Id);
            }

            return null;
        }

        public async Task<bool> VerifyBookbyTitle(string title)
        {
            bool ret = false;

            BookshelfModels.User.User? User = await UserRepos.GetUser();
            if (User?.Id != null)
            {
                Book? _book = await BookshelfRepos.Books.BooksRepos.GetBookByTitleOrGooglekey(User.Id, title, null);

                if (_book is not null)
                {
                    ret = true;
                }

            }
            return ret;

        }

        public async Task<Book?> GetBookbyTitleAndGoogleId(string title, string googleId)
        {
            BookshelfModels.User.User? User = await UserRepos.GetUser();
            if (User?.Id != null)
            {
                Book? _book = await BookshelfRepos.Books.BooksRepos.GetBookByTitleOrGooglekey(User.Id, title, googleId);

                return _book;
            }

            return null;

        }

        /// <summary>
        /// Get books situations by status
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public async Task<(List<UIBookItem>, int)> GetBookSituationByStatus(int? page, int status, string? textoBusca = null)
        {
            List<UIBookItem> listBooksItens = new();
            int total = 0;

            BookshelfModels.User.User? User = await BookshelfRepos.User.UserRepos.GetUser();
            if (User?.Id != null)
            {
                int pageSize = 10;

                List<Book> list = (await BookshelfRepos.Books.BooksRepos.GetBookSituationByStatus(status, User.Id, textoBusca));

                total = list.Count;

                if (page != null)
                    list = list.Skip((page.Value - 1) * pageSize).Take(pageSize).ToList();

                string SubtitleAndVol;

                foreach (Book book in list)
                {
                    SubtitleAndVol = "";
                    if (!string.IsNullOrEmpty(book.SubTitle))
                    {
                        SubtitleAndVol = book.SubTitle;
                    }
                    if (!string.IsNullOrEmpty(book.SubTitle) && book.Volume != null)
                    {
                        SubtitleAndVol += "; ";
                    }
                    if (book.Volume != null)
                    {
                        SubtitleAndVol += "Vol.: " + book.Volume;
                    }

                    UIBookItem bookItem = new()
                    {
                        Key = book.Id.ToString(),
                        Title = book.Title,
                        Authors = book.Authors,// + "; Ano: " + book.Year,
                        Pages = book.Pages.ToString(),
                        SubtitleAndVol = SubtitleAndVol,
                        Cover = book.Cover,
                    };

                    if ((Status)status == BookshelfModels.Books.Status.Read)
                    {
                        bookItem.Rate = book.Score > 0 ? string.Format("Avaliação pessoal: {0} de 5", book.Score.ToString()) : "";
                    }

                    listBooksItens.Add(bookItem);
                }
            }

            return (listBooksItens, total);
        }

        public async void InactivateBook(string bookKey)
        {
            Book? book = await GetBook(bookKey);

            BookshelfModels.User.User? User = await UserRepos.GetUser();
            if (book?.Id is not null && User?.Id is not null)
            {
                book.UpdatedAt = DateTime.Now;
                book.Inactive = 1;

                await BookshelfRepos.Books.BooksRepos.InactivateBook(book.Id, User.Id, book.UpdatedAt);

                if (CrossConnectivity.Current.IsConnected)
                {
                    await booksApiServices.UpdateBook(book, User);
                }
            }
        }

        public async void UpdateBookSituation(string Key, Status status, int score, string comment)
        {
            Book? book = await GetBook(Key);

            BookshelfModels.User.User? User = await UserRepos.GetUser();

            if (book is not null && User?.Id is not null)
            {
                book.UpdatedAt = DateTime.Now;
                book.Status = status;
                book.Score = score;
                book.Comment = comment;

                await BookshelfRepos.Books.BooksRepos.UpdateBook(book, User.Id);

                if (CrossConnectivity.Current.IsConnected)
                {
                    _ = booksApiServices.UpdateBook(book, User);
                }
            }
        }
    }
}
