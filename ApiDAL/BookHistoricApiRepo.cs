using ApiDAL.Interfaces;
using Models.Responses;
using System.Drawing.Printing;

namespace ApiDAL
{
    public class BookHistoricApiRepo(IHttpClientFunctions httpClientFunctions) : IBookHistoricApiRepo
    {
        public async Task<ApiResponse> GetByLastCreatedAtAsync(DateTime lastUpdate,int page)
            => await httpClientFunctions.AuthRequestAsync(RequestsTypes.Get, $"{ApiKeys.ApiAddress}/bookshelf/book/historic/bycreatedat/{lastUpdate.ToString("yyyy-MM-ddThh:mm:ss.fff")}/{page}");
    }
}
