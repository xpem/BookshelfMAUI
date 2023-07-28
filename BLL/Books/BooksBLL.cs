using BLL.Books.Api;
using Models.Books;
using LocalDbDAL.User;
using Plugin.Connectivity;

namespace BLL.Books
{
    public class BooksBLL : IBooksBLL
    {

        public async Task<Totals> GetBookshelfTotals()
        {
            Totals BTotals = new();

            Models.User? User = await UserLocalDAL.GetUser();
            if (User?.Id != null)
            {
                List<(Status, int)> list = await LocalDbDAL.Books.BooksLocalDAL.GetBookshelfTotals(User.Id);

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
            Models.User? User = await UserLocalDAL.GetUser();
            if (User?.Id != null)
                return await LocalDbDAL.Books.BooksLocalDAL.GetBook(User.Id, bookKey);

            return null;
        }

        public async Task<bool> AltBook(Book book)
        {
            Models.User? User = await UserLocalDAL.GetUser();
            if (User?.Id != null)
            {
                book.UpdatedAt = DateTime.Now;

                await LocalDbDAL.Books.BooksLocalDAL.UpdateBook(book, User.Id);
                //
                if (CrossConnectivity.Current.IsConnected)
                {
                    var resp = await BooksApiBLL.AltBook(book);

                    return resp.Success;
                }
            }
            return false;
        }

        public async Task<bool> AddBook(Book book)
        {
            book.UpdatedAt = DateTime.Now;

            Models.User? User = await UserLocalDAL.GetUser();

            if (User?.Id != null)
            {
                if (CrossConnectivity.Current.IsConnected)
                {
                    var response = await BooksApiBLL.AddBook(book);

                    if (response.Success) { book.Id = Convert.ToInt32(response.Content); }
                    else return false;
                }
                else
                {
                    book.LocalTempId = Guid.NewGuid().ToString();
                }

                await LocalDbDAL.Books.BooksLocalDAL.AddBook(book, User.Id);
                return true;
            }

            return false;
        }

        public async Task<bool> VerifyBookbyTitle(string title)
        {
            bool ret = false;

            Models.User? User = await UserLocalDAL.GetUser();
            if (User?.Id != null)
            {
                Book? _book = await LocalDbDAL.Books.BooksLocalDAL.GetBookByTitleOrGooglekey(User.Id, title, null);

                if (_book is not null)
                {
                    ret = true;
                }

            }
            return ret;

        }

        public async Task<Book?> GetBookbyTitleAndGoogleId(string title, string googleId)
        {
            Models.User? User = await UserLocalDAL.GetUser();
            if (User?.Id != null)
            {
                Book? _book = await LocalDbDAL.Books.BooksLocalDAL.GetBookByTitleOrGooglekey(User.Id, title, googleId);

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

            Models.User? User = await LocalDbDAL.User.UserLocalDAL.GetUser();
            if (User?.Id != null)
            {
                int pageSize = 10;

                List<Book> list = (await LocalDbDAL.Books.BooksLocalDAL.GetBookSituationByStatus(status, User.Id, textoBusca));

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

                    if ((Status)status == Models.Books.Status.Read)
                    {
                        bookItem.Rate = book.Score > 0 ? string.Format("Avaliação pessoal: {0} de 5", book.Score.ToString()) : "";
                    }

                    listBooksItens.Add(bookItem);
                }
            }

            return (listBooksItens, total);
        }

        public async Task InactivateBook(string bookKey)
        {
            Book? book = await GetBook(bookKey);

            Models.User? User = await UserLocalDAL.GetUser();
            if (book?.Id is not null && User?.Id is not null)
            {
                book.UpdatedAt = DateTime.Now;
                book.Inactive = 1;

                await LocalDbDAL.Books.BooksLocalDAL.InactivateBook(book.Id, User.Id, book.UpdatedAt);

                if (CrossConnectivity.Current.IsConnected)
                {
                   _ = await BooksApiBLL.AltBook(book);
                }
            }
        }

        public async Task UpdateBookSituation(string Key, Status status, int score, string comment)
        {
            Book? book = await GetBook(Key);

            Models.User? User = await UserLocalDAL.GetUser();

            if (book is not null && User?.Id is not null)
            {
                book.UpdatedAt = DateTime.Now;
                book.Status = status;
                book.Score = score;
                book.Comment = comment;

                await LocalDbDAL.Books.BooksLocalDAL.UpdateBook(book, User.Id);

                if (CrossConnectivity.Current.IsConnected)
                {
                    _ = BooksApiBLL.AltBook(book);
                }
            }
        }
    }
}
