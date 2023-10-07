using DbContextDAL;
using DBContextDAL;
using Models.Books;

namespace BLL.Books.Sync
{
    public class BookSyncBLL : IBookSyncBLL
    {
        readonly IBookApiBLL BooksApiBLL;
        private readonly IBookDAL bookDAL;

        public BookSyncBLL(IBookApiBLL booksApiBLL, IBookDAL bookDAL)
        {
            BooksApiBLL = booksApiBLL;
            this.bookDAL = bookDAL;
        }

        public async Task<(int added, int updated)> ApiToLocalSync(int uid, DateTime lastUpdate)
        {
            int added = 0, updated = 0;

            //update local database
            Models.Responses.BLLResponse respGetBooksByLastUpdate = await BooksApiBLL.GetBooksByLastUpdate(lastUpdate);

            if (respGetBooksByLastUpdate.Success && respGetBooksByLastUpdate.Content is not null)
            {
                List<Book>? BooksByLastUpdate = respGetBooksByLastUpdate.Content as List<Book>;

                if (BooksByLastUpdate is not null)
                {
                    //bookshelfDbContext.ChangeTracker.Clear();

                    foreach (Book apiBook in BooksByLastUpdate)
                    {
                        if (apiBook is null) { throw new ArgumentNullException(nameof(apiBook)); }

                        apiBook.UserId = uid;

                        DateTime? bookLastUpdate;
                        if (apiBook.Id is not null)
                            bookLastUpdate = await bookDAL.GetBookUpdatedAtByIdAsync(apiBook.Id.Value);
                        else
                            throw new ArgumentNullException(nameof(apiBook.Id));

                        if (bookLastUpdate == null && !apiBook.Inactive)
                        {
                            await bookDAL.ExecuteAddBookAsync(apiBook);
                            added++;
                        }
                        else if (apiBook.UpdatedAt > bookLastUpdate)
                        {
                            await bookDAL.ExecuteUpdateBookAsync(apiBook);
                            updated++;
                        }

                        if (lastUpdate < apiBook.UpdatedAt) lastUpdate = apiBook.UpdatedAt;
                    }
                }
            }

            return (added, updated);
        }

        /// <summary>
        /// to do - implementar sincronização in>out por fila de requisições a serem executadas.
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="lastUpdate"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<(int added, int updated)> LocalToApiSync(int uid, DateTime lastUpdate)
        {
            int added = 0, updated = 0;

            List<Book> booksList = bookDAL.GetBookByAfterUpdatedAt(uid, lastUpdate);

            //update api database
            foreach (Book book in booksList)
            {
                if (book.LocalTempId != null)
                {
                    Models.Responses.BLLResponse addBookResp = await BooksApiBLL.AddBook(book);

                    if (addBookResp.Success && addBookResp.Content is not null)
                    {
                        book.LocalTempId = null;
                        book.Id = Convert.ToInt32(addBookResp.Content);
                        await bookDAL.ExecuteUpdateBookAsync(book);
                        added++;
                    }
                    else throw new Exception($"Não foi possivel sincronizar o livro {book.Id}, res: {addBookResp.Error}");
                }
                else
                {
                    Models.Responses.BLLResponse response = await BooksApiBLL.UpdateBook(book);
                    if (response.Success)
                        updated++;
                }
            }

            return (added, updated);
        }
    }
}
