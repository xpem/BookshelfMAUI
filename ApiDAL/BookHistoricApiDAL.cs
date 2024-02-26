using ApiDAL.Interfaces;
using Models.Responses;

namespace ApiDAL
{
    public class BookHistoricApiDAL(IHttpClientFunctions httpClientFunctions) : IBookHistoricApiDAL
    {
        public async Task<ApiResponse> GetBooksHistoricByLastCreatedAt(DateTime lastUpdate)
            => await httpClientFunctions.AuthRequest(RequestsTypes.Get, ApiKeys.ApiAddress + "/bookshelf/book/historic/bycreatedat/" + lastUpdate.ToString("yyyy-MM-ddThh:mm:ss.fff"));
    }
}
