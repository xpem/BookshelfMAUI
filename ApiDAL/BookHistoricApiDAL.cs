using Models.Responses;

namespace ApiDAL
{
    public class BookHistoricApiDAL
    {
        public static async Task<ApiResponse> GetBooksHistoricByLastCreatedAt(DateTime lastUpdate)
        {
            return await HttpClientFunctions.AuthRequest(RequestsTypes.Get, ApiKeys.ApiUri + "/book/historic/bycreatedat/" + lastUpdate.ToString("yyyy-MM-ddThh:mm:ss.fff"));
        }
    }
}
