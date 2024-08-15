using Models.OperationQueue;

namespace Repositories.Interfaces
{
    public interface IOperationQueueDAL
    {
        Task<List<ApiOperation>> GetPendingOperationsByStatusAsync(OperationStatus operationStatus);

        Task InsertOperationInQueueAsync(ApiOperation apiOperation);

        Task UpdateOperationStatusAsync(OperationStatus operationStatus, int operationId);

        Task<bool> CheckIfHasPendingOperationWithObjectId(string objectId);

        Task<bool> CheckIfHasPendingOperation();
    }
}
