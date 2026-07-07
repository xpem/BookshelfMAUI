using Models.Responses;

namespace Services.Books.Historic.Interfaces
{
    public interface IBookHistoricApiServices
    {
        Task<ServiceResponse> GetByLastCreatedAtAsync(DateTime lastCreatedAt, int page);
    }
}
