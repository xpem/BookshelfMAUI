using Models.Responses;

namespace ApiRepo.Interfaces
{
    public interface IBookHistoricApiRepo
    {
        Task<ApiResponse> GetByLastCreatedAtAsync(DateTime lastUpdate, int page);
    }
}
