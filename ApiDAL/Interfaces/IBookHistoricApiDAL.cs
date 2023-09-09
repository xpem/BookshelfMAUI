using Models.Responses;

namespace ApiDAL.Interfaces
{
    public interface IBookHistoricApiDAL
    {
        Task<ApiResponse> GetBooksHistoricByLastCreatedAt(DateTime lastUpdate);
    }
}
