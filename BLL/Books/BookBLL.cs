using Repositories.Interfaces;
using Models.Books;
using Models.Responses;
using Models.DTOs;
using Services.Books.Interfaces;

namespace Services.Books
{
    public class BookBLL(IBookApiService booksApiBLL, IBookRepo bookDAL, IBooksOperationService booksOperationBLL) : IBookBLL
    {
        public async Task<Totals> GetBookshelfTotalsAsync(int uid)
        {
            List<TotalBooksGroupedByStatus>? list = await bookDAL.GetTotalBooksGroupedByStatusAsync(uid);

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

        public async Task<Book?> GetBookAsync(int uid, int localId) => await bookDAL.GetBookByLocalIdAsync(uid, localId);

        public async Task<BLLResponse> UpdateBookAsync(int uid, bool isOn, Book book)
        {
            if (!await CheckIfExistsBookWithSameTitleAsync(uid, book.Title, book.LocalId))
            {
                book.UpdatedAt = DateTime.Now;
                book.UserId = uid;

                await bookDAL.UpdateAsync(book);

                //
                _ = ApiUpdateBook(book, isOn);

                return new BLLResponse() { Success = true };
            }
            else return new BLLResponse() { Success = false, Content = "Livro com este título já cadastrado." };
        }

        public async Task<BLLResponse> AddBookAsync(int uid, bool isOn, Book book)
        {
            book.UpdatedAt = DateTime.Now;
            book.UserId = uid;

            var bookResponse = await bookDAL.GetBookByTitleOrGoogleIdAsync(uid, book.Title);

            if (bookResponse is null)
            {
                await bookDAL.CreateAsyn(book);

                if (isOn)
                {
                    BLLResponse response = await booksApiBLL.CreateAsync(book);

                    if (response.Success)
                    {
                        book.Id = Convert.ToInt32(response.Content);
                        await bookDAL.UpdateAsync(book);
                    }
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

                if (book.Status is not null && (Status)book.Status == Status.Read)                
                    bookItem.Rate = book.Score > 0 ? book.Score.Value : 0;                

                listBooksItens.Add(bookItem);
            }

            return listBooksItens;
        }

        public async Task InactivateBookAsync(int uid, bool isOn, int localId)
        {
            Book? book = await GetBookAsync(uid, localId);

            if (book is not null)
            {
                book.UpdatedAt = DateTime.Now;
                book.UserId = uid;
                book.Inactive = true;

                await bookDAL.ExecuteInactivateBookAsync(localId, book.UserId);

                _ = ApiUpdateBook(book, isOn);
            }
        }

        public async Task UpdateBookSituationAsync(int uid, bool isOn, int localId, Status status, int score, string comment)
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

                    _ = ApiUpdateBook(book, isOn);
                }
            }
            catch (Exception ex) { throw ex; }
        }

        private async Task ApiUpdateBook(Book book, bool isOn)
        {
            if (isOn)
            {
                BLLResponse resp = await booksApiBLL.UpdateAsync(book);

                if (!resp.Success) throw new Exception($"Could not be possible update book, book id: {book.Id}, Erro: {resp.Content?.ToString()} ");
            }
            else _ = booksOperationBLL.InsertOperationUpdateBookAsync(book);
        }

        public async Task<Book?> GetBookbyTitleOrGoogleIdAsync(int uid, string title, string googleId)
            => await bookDAL.GetBookByTitleOrGoogleIdAsync(uid, title, googleId);

        public async Task<bool> CheckIfExistsBookWithSameTitleAsync(int uid, string title, int? localId)
            => await bookDAL.CheckIfExistsBookWithSameTitleAsync(uid, title, localId);
    }
}
