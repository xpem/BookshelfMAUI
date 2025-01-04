using Models.DTOs;
using Models.DTOs.OperationQueue;
using Repositories.Interfaces;
using System.Text.Json;

namespace Services.Books
{
    public interface IBooksOperationService
    {
        Task InsertOperationInsertBookAsync(Book book);

        Task InsertOperationUpdateBookAsync(Book book);

        Task<bool> CheckIfHasPendingOperationsWithBookId(int bookId);

        Task<bool> CheckIfHasPendingOperation();
    }

    public class BooksOperationService(IOperationQueueRepo operationQueueDAL) : ApiOperationBaseService(operationQueueDAL), IBooksOperationService
    {
        public async Task InsertOperationInsertBookAsync(Book book) =>
            await InsertOperationAsync(JsonSerializer.Serialize(book), book.LocalId.ToString(), ExecutionType.Insert);

        public async Task InsertOperationUpdateBookAsync(Book book) =>
           await InsertOperationAsync(JsonSerializer.Serialize(book), book.LocalId.ToString() ?? throw new ArgumentNullException(), ExecutionType.Update);

        public async Task<bool> CheckIfHasPendingOperationsWithBookId(int bookId) => await operationQueueDAL.CheckIfHasPendingOperationWithObjectId(bookId.ToString());

        public async Task<bool> CheckIfHasPendingOperation() => await operationQueueDAL.CheckIfHasPendingOperation();
    }
}
