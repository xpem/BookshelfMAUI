using DBContextDAL;
using Models.Books;
using System.Text.Json;

namespace BLL.Books
{
    public interface IBooksOperationBLL
    {
        Task InsertOperationInsertBookAsync(Book book);
        Task InsertOperationUpdateBookAsync(Book book);
    }

    public class BooksOperationBLL(BookshelfDbContext bookshelfDbContext) : OperationBaseBLL(bookshelfDbContext), IBooksOperationBLL
    {
        public async Task InsertOperationInsertBookAsync(Models.Books.Book book) =>
            await InsertOperationAsync(Models.Responses.RequestsTypes.Post, "/bookshelf/book", JsonSerializer.Serialize(book),
                book.LocalTempId ?? throw new ArgumentNullException(), Models.OperationQueue.ExecutionType.Insert);

        public async Task InsertOperationUpdateBookAsync(Models.Books.Book book) =>
           await InsertOperationAsync(Models.Responses.RequestsTypes.Put, "/bookshelf/book/" + book.Id, JsonSerializer.Serialize(book),
               book.Id.ToString() ?? throw new ArgumentNullException(), Models.OperationQueue.ExecutionType.Update);
    }
}
