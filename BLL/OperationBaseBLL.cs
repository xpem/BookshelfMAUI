using DbContextDAL;
using Models.OperationQueue;

namespace BLL
{
    public interface IOperationBaseBLL
    {
        Task InsertOperationAsync(string jsonContent, string objectId, ExecutionType executionType);
    }

    public class OperationBaseBLL(IOperationQueueDAL operationQueueDAL)
    {
        protected async Task InsertOperationAsync(string jsonContent, string objectId, ExecutionType executionType)
        {
            Models.OperationQueue.ApiOperation apiOperation = new()
            {
                CreatedAt = DateTime.Now,
                ObjectType = Models.OperationQueue.ObjectType.Book,
                Status = Models.OperationQueue.OperationStatus.Pending,
                UpdatedAt = DateTime.Now,
                Content = jsonContent,
                ObjectId = objectId,
                ExecutionType = executionType
            };

            await operationQueueDAL.InsertOperationInQueueAsync(apiOperation);
        }
    }
}
