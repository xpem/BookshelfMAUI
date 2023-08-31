using DBContextDAL;
using Models.Books;

namespace BLL.Books.Sync
{
    public class BookSyncBLL : IBookSyncBLL
    {
        readonly IBookApiBLL BooksApiBLL;
        private readonly BookshelfDbContext bookshelfDbContext;

        public BookSyncBLL(IBookApiBLL booksApiBLL, BookshelfDbContext bookshelfDbContext)
        {
            BooksApiBLL = booksApiBLL;
            this.bookshelfDbContext = bookshelfDbContext;
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
                    DateTime? bookLastUpdate = null;

                    foreach (Book book in BooksByLastUpdate)
                    {
                        if (book is null) { throw new ArgumentNullException(nameof(book)); }
                        book.UserId = uid;

                        if (book.Id is not null)
                            bookLastUpdate = bookshelfDbContext.Book.Where(x => x.Id.Equals(book.Id)).FirstOrDefault()?.UpdatedAt;
                        else throw new ArgumentNullException(nameof(book.Id));

                        //else if (!string.IsNullOrEmpty(book.Title))
                        //    bookLastUpdate = (await BooksDbDAL.GetBookByTitle(book.Title))?.UpdatedAt;

                        if (bookLastUpdate == null && !book.Inactive)
                        {
                            await bookshelfDbContext.Book.AddAsync(book);
                            added++;
                        }
                        else if (book.UpdatedAt > bookLastUpdate)
                        {
                            bookshelfDbContext.Book.Update(book);
                            updated++;
                        }

                        await bookshelfDbContext.SaveChangesAsync();

                        if (lastUpdate < book.UpdatedAt) lastUpdate = book.UpdatedAt;
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

            List<Book> booksList = bookshelfDbContext.Book.Where(x => x.UserId == uid && x.UpdatedAt > lastUpdate).ToList();

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

                        bookshelfDbContext.Book.Update(book);
                        added++;
                    }
                    else throw new Exception($"Não foi possivel sincronizar o livro {book.Id}, res: {addBookResp.Error}");
                }
                else
                {
                    var response = await BooksApiBLL.UpdateBook(book);
                    if (response.Success)
                        updated++;
                }

                await bookshelfDbContext.SaveChangesAsync();

            }

            return (added, updated);


        }
    }
}
