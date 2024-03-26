using DbContextDAL;
using DBContextDAL;
using Models.Books;
using System.Text.Json;

namespace BLL.Books
{
    public interface IBooksOperationBLL
    {
        Task InsertOperationInsertBookAsync(Book book);

        Task InsertOperationUpdateBookAsync(Book book);

        Task<bool> CheckIfHasPendingOperationsWithBookId(int bookId);
    }

    public class BooksOperationBLL(IOperationQueueDAL operationQueueDAL) : ApiOperationBaseBLL(operationQueueDAL), IBooksOperationBLL
    {
        public async Task InsertOperationInsertBookAsync(Models.Books.Book book) =>
            await InsertOperationAsync(JsonSerializer.Serialize(book), book.LocalId.ToString(), Models.OperationQueue.ExecutionType.Insert);

        public async Task InsertOperationUpdateBookAsync(Models.Books.Book book) =>
           await InsertOperationAsync(JsonSerializer.Serialize(book), book.LocalId.ToString() ?? throw new ArgumentNullException(), Models.OperationQueue.ExecutionType.Update);

        public async Task<bool> CheckIfHasPendingOperationsWithBookId(int bookId) => await operationQueueDAL.CheckIfHasPendingOperationWithObjectId(bookId.ToString());
    }
}
