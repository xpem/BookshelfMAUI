using DbContextDAL;
using Models.Books;
using Models.Responses;
using Plugin.Connectivity;

namespace BLL.Books
{
    public class BooksBLL(IBookApiBLL booksApiBLL, IBookDAL bookDAL, IUserDAL userDAL) : IBooksBLL
    {

        public async Task<Totals> GetBookshelfTotalsAsync(int uid) => await bookDAL.GetTotalBooksGroupedByStatusAsync(uid);

        public async Task<Book?> GetBook(int localId) => await bookDAL.GetBookByLocalIdAsync(userDAL.GetUidAsync().Result, localId);

        public async Task<BLLResponse> UpdateBook(Book book)
        {
            Book? bookResponse = await bookDAL.GetBookByTitleAsync(await userDAL.GetUidAsync(), book.Title);

            if (bookResponse != null && bookResponse.LocalId.Equals(book.LocalId))
            {
                book.UpdatedAt = DateTime.Now;
                book.UserId = await userDAL.GetUidAsync();

                await bookDAL.ExecuteUpdateBookAsync(book);

                //
                if (CrossConnectivity.Current.IsConnected)
                {
                    BLLResponse resp = await booksApiBLL.UpdateBook(book);

                    if (!resp.Success)
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

        public async Task<BLLResponse> AddBook(Book book)
        {
            book.UpdatedAt = DateTime.Now;
            int uid = await userDAL.GetUidAsync();

            Book? bookResponse = Task.Run(() => bookDAL.GetBookByTitleAsync(uid, book.Title)).Result;

            if (bookResponse == null)
            {
                if (CrossConnectivity.Current.IsConnected)
                {
                    BLLResponse response = await booksApiBLL.AddBook(book);

                    if (response.Success) { book.Id = Convert.ToInt32(response.Content); }
                    else
                    {
                        if (response.Content is not null)
                            return new BLLResponse() { Success = false, Content = response.Content.ToString() };
                        else return new BLLResponse() { Success = false };
                    }
                }
                else
                    book.LocalTempId = Guid.NewGuid().ToString();

                book.UserId = uid;

                bookDAL.ExecuteAddBook(book);

                return new BLLResponse() { Success = true };
            }
            else return new BLLResponse() { Success = false, Content = "Livro com este título já cadastrado." };

        }

        /// <summary>
        /// Get books situations by status
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public async Task<(List<UIBookItem>, int)> GetBooksByStatusAsync(int? page, int status, string? textoBusca = null)
        {
            List<UIBookItem> listBooksItens = [];
            int total = 0;

            int pageSize = 10;
            List<Book> list = [];

            if (status > 0)
                list = bookDAL.GetBooksByStatus(await userDAL.GetUidAsync(), (Status)status);
            else
                list = await bookDAL.GetBooks(await userDAL.GetUidAsync());

            if (list.Count > 0 && !string.IsNullOrEmpty(textoBusca))
                list = list.Where(x => x.Title != null && x.Title.Contains(textoBusca, StringComparison.CurrentCultureIgnoreCase)).ToList();

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


            return (listBooksItens, total);
        }

        public async Task InactivateBookAsync(int localId)
        {
            Book? book = await GetBook(localId);

            if (book is not null)
            {
                book.UpdatedAt = DateTime.Now;
                book.UserId = await userDAL.GetUidAsync();
                book.Inactive = true;

                await bookDAL.ExecuteInactivateBookAsync(localId, book.UserId);

                if (CrossConnectivity.Current.IsConnected)
                    await booksApiBLL.UpdateBook(book);
            }
        }

        public async Task UpdateBookSituationAsync(int localId, Status status, int score, string comment)
        {
            try
            {
                Book? book = await GetBook(localId);

                if (book is not null)
                {
                    book.UpdatedAt = DateTime.Now;
                    book.Status = status;
                    book.Score = score;
                    book.Comment = comment;
                    book.UserId = await userDAL.GetUidAsync();

                    await bookDAL.ExecuteUpdateBookStatusAsync(localId, status, score, comment, book.UserId);

                    if (CrossConnectivity.Current.IsConnected)
                        _ = booksApiBLL.UpdateBook(book);
                }
            }
            catch (Exception ex) { throw ex; }
        }

        public Book? GetBookbyTitleOrGoogleId(string title, string googleId)
            => bookDAL.GetBookByTitleOrGoogleId(userDAL.GetUidAsync().Result, title, googleId);
    }
}
