using DBContextDAL;
using Microsoft.EntityFrameworkCore;
using Models.OperationQueue;

namespace BLL
{
    public interface IOperationBaseBLL
    {
        Task<List<ApiOperation>> GetPendingOperationsByStatusAsync(OperationStatus operationStatus);

        Task UpdateOperationBookStatusAsync(OperationStatus operationStatus, int operationId);
    }

    public class OperationBaseBLL(BookshelfDbContext bookshelfDbContext) : IOperationBaseBLL
    {
        protected async Task InsertOperationAsync(Models.Responses.RequestsTypes requestsType, string Url, string jsonContent, string objectId, ExecutionType executionType)
        {
            Models.OperationQueue.ApiOperation apiOperation = new()
            {
                CreatedAt = DateTime.Now,
                ObjectType = Models.OperationQueue.ObjectType.Book,
                Status = Models.OperationQueue.OperationStatus.Pending,
                UpdatedAt = DateTime.Now,
                RequestType = requestsType,
                Url = Url,
                Content = jsonContent,
                ObjectId = objectId,
                ExecutionType = executionType
            };

            bookshelfDbContext.ApiOperationQueue.Add(apiOperation);

            await bookshelfDbContext.SaveChangesAsync();

            bookshelfDbContext.ChangeTracker?.Clear();
        }

        public async Task<List<ApiOperation>> GetPendingOperationsByStatusAsync(OperationStatus operationStatus) =>
            await bookshelfDbContext.ApiOperationQueue.Where(x => x.Status == operationStatus).OrderBy(x => x.CreatedAt).ToListAsync();

        public async Task UpdateOperationBookStatusAsync(OperationStatus operationStatus, int operationId) =>
            await bookshelfDbContext.ApiOperationQueue.Where(x => x.Id == operationId)
            .ExecuteUpdateAsync(y => y
            .SetProperty(z => z.Status, operationStatus));
    }
}
