using Models.Responses;

namespace BLL.Books.Historic.Interfaces
{
    public interface IBookHistoricApiServices
    {
        Task<BLLResponse> GetByLastCreatedAtAsync(DateTime lastCreatedAt, int page);
    }
}
