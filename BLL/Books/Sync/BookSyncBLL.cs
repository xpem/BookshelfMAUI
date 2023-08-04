using Models.Books;
using Models;
using LocalDbDAL.Books;

namespace BLL.Books.Sync
{
    public class BookSyncBLL : IBookSyncBLL
    {
        readonly IBooksApiBLL BooksApiBLL;
        readonly IBookLocalDAL BookLocalDAL;

        public BookSyncBLL(IBooksApiBLL booksApiBLL, IBookLocalDAL booksLocalDAL)
        {
            BooksApiBLL = booksApiBLL;
            BookLocalDAL = booksLocalDAL;
        }

        public async Task ApiToLocalSync(int uid, DateTime lastUpdate)
        {
            //update local database
            Models.Responses.BLLResponse respGetBooksByLastUpdate = await BooksApiBLL.GetBooksByLastUpdate(lastUpdate);

            if (respGetBooksByLastUpdate.Success && respGetBooksByLastUpdate.Content is not null)
            {
                List<Book>? BooksByLastUpdate = respGetBooksByLastUpdate.Content as List<Book>;

                if (BooksByLastUpdate is not null)
                {
                    foreach (Book book in BooksByLastUpdate)
                    {
                        await BookLocalDAL.AddOrUpdateBook(book, uid);

                        if (lastUpdate < book.UpdatedAt) lastUpdate = book.UpdatedAt;
                    }
                }
            }
        }

        /// <summary>
        /// to do - implementar sincronização in>out por fila de requisições a serem executadas.
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="lastUpdate"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task LocalToApiSync(int uid, DateTime lastUpdate)
        {
            List<Book> booksList = await BookLocalDAL.GetBooksByLastUpdate(uid, lastUpdate);

            //update api database
            foreach (Book book in booksList)
            {
                //if the book has a local temporary Guid key, register it in the firebase
                if (book.LocalTempId != null)
                {
                    //define the key has a null for register the book in firebase
                    Models.Responses.BLLResponse addBookResp = await BooksApiBLL.AddBook(book);

                    if (addBookResp.Success && addBookResp.Content is not null)
                    {
                        string localTempId = book.LocalTempId;
                        book.LocalTempId = null;
                        await BookLocalDAL.UpdateBookId(localTempId, Convert.ToString(addBookResp.Content), uid);
                    }
                    else throw new Exception($"Não foi possivel sincronizar o livro {book.Id}, res: {addBookResp.Error}");
                }
                else
                    await BookLocalDAL.UpdateBook(book, uid);
            }
        }
    }
}
