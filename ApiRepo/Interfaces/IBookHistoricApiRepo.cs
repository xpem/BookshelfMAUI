using Models.Responses;

namespace ApiRepo.Interfaces
{
    public interface IBookHistoricApiRepo
    {
        Task<ApiResp> GetByLastCreatedAtAsync(DateTime lastUpdate, int page);
    }
}
