using DbContextDAL;
using Models.Books;
using Models.Responses;
using Plugin.Connectivity;

namespace BLL.Books
{
    public class BooksBLL(IBookApiBLL booksApiBLL, IBookDAL bookDAL, IBooksOperationBLL booksOperationBLL) : IBooksBLL
    {

        public async Task<Totals> GetBookshelfTotalsAsync(int uid)
        {
            var list = await bookDAL.GetTotalBooksGroupedByStatusAsync(uid);

            Totals totals = new();

            if (list is not null && list.Count > 0)
            {
                var illRead = list.FirstOrDefault(x => x.Status == Status.IllRead);
                var reading = list.FirstOrDefault(x => x.Status == Status.Reading);
                var read = list.FirstOrDefault(x => x.Status == Status.Read);
                var interrupted = list.FirstOrDefault(x => x.Status == Status.Interrupted);

                totals.IllRead = illRead is not null ? illRead.Count : 0;
                totals.Reading = reading is not null ? reading.Count : 0;
                totals.Read = read is not null ? read.Count : 0;
                totals.Interrupted = interrupted is not null ? interrupted.Count : 0;
            }
            else
            {
                totals.IllRead = totals.Reading = totals.Read = totals.Interrupted = 0;
            }

            return totals;
        }

        private static bool IsConnected() => CrossConnectivity.Current.IsConnected;

        public async Task<Book?> GetBookAsync(int uid, int localId) => await bookDAL.GetBookByLocalIdAsync(uid, localId);

        public async Task<BLLResponse> UpdateBookAsync(int uid, Book book)
        {
            Book? bookResponse = await bookDAL.GetBookByTitleAsync(uid, book.Title);

            if (bookResponse != null && bookResponse.LocalId.Equals(book.LocalId))
            {
                book.UpdatedAt = DateTime.Now;
                book.UserId = uid;

                await bookDAL.ExecuteUpdateBookAsync(book);

                //
                _ = ApiUpdateBook(book);

                return new BLLResponse() { Success = true };
            }
            else return new BLLResponse() { Success = false, Content = "Livro com este título já cadastrado." };
        }

        public async Task<BLLResponse> AddBookAsync(int uid, Book book)
        {
            book.UpdatedAt = DateTime.Now;

            Book? bookResponse = await bookDAL.GetBookByTitleAsync(uid, book.Title);

            if (bookResponse == null)
            {
                if (IsConnected())
                {
                    BLLResponse response = await booksApiBLL.AddBookAsync(book);

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
                    _ = booksOperationBLL.InsertOperationInsertBookAsync(book);
                }
                book.UserId = uid;

                await bookDAL.ExecuteAddBookAsync(book);

                return new BLLResponse() { Success = true };
            }
            else return new BLLResponse() { Success = false, Content = "Livro com este título já cadastrado." };

        }

        /// <summary>
        /// Get books situations by status
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public async Task<List<UIBookItem>> GetBooksByStatusAsync(int uid, int page, int status, string? textoBusca = null)
        {
            List<UIBookItem> listBooksItens = [];
            int total = 0;

            List<Book> list = [];

            if (status > 0)
                list = await bookDAL.GetBooksByStatusAsync(uid, (Status)status, page);
            else
                list = await bookDAL.GetBooksAsync(uid, page);

            if (list.Count > 0 && !string.IsNullOrEmpty(textoBusca))
                list = list.Where(x => x.Title != null && x.Title.Contains(textoBusca, StringComparison.CurrentCultureIgnoreCase)).ToList();

            total = list.Count;

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
                    Id = book.LocalId,
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


            return listBooksItens;
        }

        public async Task InactivateBookAsync(int uid, int localId)
        {
            Book? book = await GetBookAsync(uid, localId);

            if (book is not null)
            {
                book.UpdatedAt = DateTime.Now;
                book.UserId = uid;
                book.Inactive = true;

                await bookDAL.ExecuteInactivateBookAsync(localId, book.UserId);

                _ = ApiUpdateBook(book);
            }
        }

        public async Task UpdateBookSituationAsync(int uid, int localId, Status status, int score, string comment)
        {
            try
            {
                Book? book = await GetBookAsync(uid, localId);

                if (book is not null)
                {
                    book.UpdatedAt = DateTime.Now;
                    book.Status = status;
                    book.Score = score;
                    book.Comment = comment;
                    book.UserId = uid;

                    await bookDAL.ExecuteUpdateBookStatusAsync(localId, status, score, comment, book.UserId);

                    _ = ApiUpdateBook(book);
                }
            }
            catch (Exception ex) { throw ex; }
        }

        private async Task ApiUpdateBook(Book book)
        {
            if (IsConnected())
            {
                BLLResponse resp = await booksApiBLL.UpdateBookAsync(book);

                if (!resp.Success) throw new Exception($"Could not be possible update book, book id: {book.Id}, Erro: {resp.Content?.ToString()} ");
            }
            else _ = booksOperationBLL.InsertOperationUpdateBookAsync(book);
        }

        public Book? GetBookbyTitleOrGoogleId(int uid, string title, string googleId)
            => bookDAL.GetBookByTitleOrGoogleId(uid, title, googleId);
    }
}
