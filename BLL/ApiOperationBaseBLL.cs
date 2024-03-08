using DbContextDAL;
using Models.OperationQueue;

namespace BLL
{
    public interface IApiOperationBaseBLL
    {
        Task InsertOperationAsync(string jsonContent, string objectId, ExecutionType executionType);
    }

    public class ApiOperationBaseBLL(IOperationQueueDAL operationQueueDAL)
    {
        protected async Task InsertOperationAsync(string jsonContent, string objectId, ExecutionType executionType)
        {
            DateTime dateTimeNow = DateTime.Now;

            Models.OperationQueue.ApiOperation apiOperation = new()
            {
                CreatedAt = dateTimeNow,
                ObjectType = Models.OperationQueue.ObjectType.Book,
                Status = Models.OperationQueue.OperationStatus.Pending,
                UpdatedAt = dateTimeNow,
                Content = jsonContent,
                ObjectId = objectId,
                ExecutionType = executionType
            };

            await operationQueueDAL.InsertOperationInQueueAsync(apiOperation);
        }
    }
}
