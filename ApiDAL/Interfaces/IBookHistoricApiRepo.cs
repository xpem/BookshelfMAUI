using Models.Responses;

namespace ApiDAL.Interfaces
{
    public interface IBookHistoricApiRepo
    {
        Task<ApiResponse> GetByLastCreatedAtAsync(DateTime lastUpdate, int page);
    }
}
