using ApiDAL.Interfaces;
using Models.Responses;

namespace ApiDAL
{
    public class BookHistoricApiDAL : IBookHistoricApiDAL
    {
        readonly IHttpClientFunctions HttpClientFunctions;

        public BookHistoricApiDAL(IHttpClientFunctions httpClientFunctions) { HttpClientFunctions = httpClientFunctions; }

        public async Task<ApiResponse> GetBooksHistoricByLastCreatedAt(DateTime lastUpdate)
            => await HttpClientFunctions.AuthRequest(RequestsTypes.Get, ApiKeys.ApiUri + "/book/historic/bycreatedat/" + lastUpdate.ToString("yyyy-MM-ddThh:mm:ss.fff"));
    }
}
