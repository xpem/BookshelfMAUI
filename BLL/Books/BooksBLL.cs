using DBContextDAL;
using Microsoft.EntityFrameworkCore;
using Models.Books;
using Models.Responses;
using Plugin.Connectivity;

namespace BLL.Books
{
    public class BooksBLL : IBooksBLL
    {
        readonly IBookApiBLL BooksApiBLL;
        private readonly BookshelfDbContext bookshelfDbContext;
        readonly Models.User User;

        public BooksBLL(IBookApiBLL booksApiBLL, BookshelfDbContext bookshelfDbContext)
        {
            BooksApiBLL = booksApiBLL;
            this.bookshelfDbContext = bookshelfDbContext;

            User = bookshelfDbContext.User.First();
        }

        public Totals GetBookshelfTotals()
        {
            Totals BTotals = new();

            var list = bookshelfDbContext.Book.Where(x => x.UserId == User.Id && x.Inactive == false).GroupBy(x => x.Status).Select(x => new { status = x.Key, count = x.Count() }).ToList();

            if (list.Count > 0)
            {
                var illRead = list.FirstOrDefault(x => x.status == Status.IllRead);
                var reading = list.FirstOrDefault(x => x.status == Status.Reading);
                var read = list.FirstOrDefault(x => x.status == Status.Read);
                var interrupted = list.FirstOrDefault(x => x.status == Status.Interrupted);

                BTotals.IllRead = illRead is not null ? illRead.count : 0;
                BTotals.Reading = reading is not null ? reading.count : 0;
                BTotals.Read = read is not null ? read.count : 0;
                BTotals.Interrupted = interrupted is not null ? interrupted.count : 0;
            }
            else
            {
                BTotals.IllRead = BTotals.Reading = BTotals.Read = BTotals.Interrupted = 0;
            }

            return BTotals;
        }

        public async Task<Book?> GetBook(int bookId) => await bookshelfDbContext.Book.Where(x => x.UserId == User.Id && x.Id == bookId).FirstOrDefaultAsync();

        public async Task<BLLResponse> UpdateBook(Book book)
        {
            Book? bookResponse = Task.Run(() => GetBookByTitle(book.Title)).Result;

            if (bookResponse == null)
            {
                book.UpdatedAt = DateTime.Now;
                book.UserId = User.Id;

                bookshelfDbContext.Update(book);
                await bookshelfDbContext.SaveChangesAsync();

                //
                if (CrossConnectivity.Current.IsConnected)
                {
                    BLLResponse resp = await BooksApiBLL.UpdateBook(book);

                    if (resp.Success) { book.Id = Convert.ToInt32(resp.Content); }
                    else
                    {
                        if (resp.Content is not null)
                            return new BLLResponse() { Success = false, Content = resp.Content.ToString() };
                        else return new BLLResponse() { Success = false };
                    }
                }

                return new BLLResponse() { Success = true };
            }
            else return new BLLResponse() { Success = false, Content = "Livro com este título já cadastrado." };

        }

        private Book? GetBookByTitle(string title) => bookshelfDbContext.Book.Where(x => x.UserId == User.Id && x.Title != null && x.Title.ToLower().Equals(title.ToLower())).FirstOrDefault();

        public async Task<BLLResponse> AddBook(Book book)
        {
            book.UpdatedAt = DateTime.Now;

            Book? bookResponse = Task.Run(() => GetBookByTitle(book.Title)).Result;

            if (bookResponse == null)
            {
                if (CrossConnectivity.Current.IsConnected)
                {
                    BLLResponse response = await BooksApiBLL.AddBook(book);

                    if (response.Success) { book.Id = Convert.ToInt32(response.Content); }
                    else
                    {
                        if (response.Content is not null)
                            return new BLLResponse() { Success = false, Content = response.Content.ToString() };
                        else return new BLLResponse() { Success = false };
                    }
                }
                else
                {
                    book.LocalTempId = Guid.NewGuid().ToString();
                }

                book.UserId = User.Id;
                bookshelfDbContext.Add(book);
                bookshelfDbContext.SaveChanges();

                return new BLLResponse() { Success = true };
            }
            else return new BLLResponse() { Success = false, Content = "Livro com este título já cadastrado." };

        }

        //public bool VerifyBookbyTitle(string title)
        //{
        //    bool ret = false;

        //    Models.User? User = bookshelfDbContext.User.FirstOrDefault();
        //    if (User?.Id != null)
        //    {
        //        Book? _book = bookshelfDbContext.Book.Where(x => x.UserId == User.Id && x.Title != null && x.Title.Equals(title, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();

        //        if (_book is not null)
        //            ret = true;
        //    }

        //    return ret;

        //}

        public Book? GetBookbyTitleOrGoogleId(string title, string googleId)
        {
            try
            {
                return bookshelfDbContext.Book.Where(x => x.UserId == User.Id && ((x.Title != null &&
                   x.Title.ToLower().Equals(title.ToLower())) || (x.GoogleId != null && x.GoogleId.Equals(googleId)))).FirstOrDefault();
            }
            catch (Exception ex) { throw ex; }
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

            int pageSize = 10;
            List<Book> list = new();

            if (status > 0)
                list = await bookshelfDbContext.Book.Where(x => x.UserId == User.Id && x.Status == (Status)status && x.Inactive == false).OrderBy(x => x.UpdatedAt).ToListAsync();
            else
                list = await bookshelfDbContext.Book.Where(x => x.UserId == User.Id && x.Inactive == false).OrderBy(x => x.UpdatedAt).ToListAsync();

            if (list.Count > 0 && !string.IsNullOrEmpty(textoBusca))
                list = list.Where(x => x.Title != null && x.Title.ToLower().Equals(textoBusca.ToLower())).ToList();

            total = list.Count;

            if (page != null)
                list = list.Skip((page.Value - 1) * pageSize).Take(pageSize).ToList();

            string SubtitleAndVol;

            foreach (Book book in list)
            {
                SubtitleAndVol = "";

                if (!string.IsNullOrEmpty(book.SubTitle))
                    SubtitleAndVol = book.SubTitle;
                if (!string.IsNullOrEmpty(book.SubTitle) && book.Volume != null)
                    SubtitleAndVol += "; ";
                if (book.Volume != null)
                    SubtitleAndVol += "Vol.: " + book.Volume;

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


            return (listBooksItens, total);
        }

        public async Task InactivateBook(int bookId)
        {
            Book? book = await GetBook(bookId);

            if (book?.Id is not null)
            {
                book.UpdatedAt = DateTime.Now;
                book.UserId = User.Id;
                book.Inactive = true;

                bookshelfDbContext.Update(book);

                bookshelfDbContext.SaveChanges();

                if (CrossConnectivity.Current.IsConnected)
                    await BooksApiBLL.UpdateBook(book);
            }
        }

        public async Task UpdateBookSituation(int bookId, Status status, int score, string comment)
        {
            try
            {
                Book? book = await GetBook(bookId);

                if (book is not null && User?.Id is not null)
                {
                    book.UpdatedAt = DateTime.Now;
                    book.Status = status;
                    book.Score = score;
                    book.Comment = comment;
                    book.UserId = User.Id;

                    bookshelfDbContext.Update(book);

                    await bookshelfDbContext.SaveChangesAsync();

                    if (CrossConnectivity.Current.IsConnected)
                    {
                        _ = BooksApiBLL.UpdateBook(book);
                    }
                }
            }
            catch (Exception ex) { throw ex; }
        }
    }
}
