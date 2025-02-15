using Models.DTOs.OperationQueue;

namespace Repos.Interfaces
{
    public interface IOperationQueueRepo
    {
        Task<List<ApiOperation>> GetPendingOperationsByStatusAsync(ApiOperationStatus operationStatus);

        Task InsertOperationInQueueAsync(ApiOperation apiOperation);

        Task UpdateOperationStatusAsync(ApiOperationStatus operationStatus, int operationId);

        Task<bool> CheckIfHasPendingOperationWithObjectId(string objectId);

        Task<bool> CheckIfHasPendingOperation();
    }
}
