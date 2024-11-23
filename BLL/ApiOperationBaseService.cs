using Models.DTOs.OperationQueue;
using Repositories.Interfaces;

namespace Services
{
    public interface IApiOperationBaseBLL
    {
        Task InsertOperationAsync(string jsonContent, string objectId, ExecutionType executionType);
    }

    public class ApiOperationBaseService(IOperationQueueDAL operationQueueDAL)
    {
        protected async Task InsertOperationAsync(string jsonContent, string objectId, ExecutionType executionType)
        {
            DateTime dateTimeNow = DateTime.Now;

            ApiOperation apiOperation = new()
            {
                CreatedAt = dateTimeNow,
                ObjectType = ObjectType.Book,
                Status = OperationStatus.Pending,
                UpdatedAt = dateTimeNow,
                Content = jsonContent,
                ObjectId = objectId,
                ExecutionType = executionType
            };

            await operationQueueDAL.InsertOperationInQueueAsync(apiOperation);
        }
    }
}
