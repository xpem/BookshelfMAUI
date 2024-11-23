using Models.Responses;

namespace Services.Books.Historic.Interfaces
{
    public interface IBookHistoricApiServices
    {
        Task<BLLResponse> GetByLastCreatedAtAsync(DateTime lastCreatedAt, int page);
    }
}
