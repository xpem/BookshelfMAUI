using ApiDAL.Interfaces;
using BLL.Books.Historic.Interfaces;
using BLL.Handlers;
using Models.Responses;

namespace BLL.Books.Historic
{
    public class BookHistoricApiServices(IBookHistoricApiRepo bookHistoricApiDAL) : IBookHistoricApiServices
    {
        public async Task<BLLResponse> GetByLastCreatedAtAsync(DateTime lastCreatedAt, int page)
        {
            ApiResponse resp = await bookHistoricApiDAL.GetByLastCreatedAtAsync(lastCreatedAt, page);

            return ApiResponseHandler.Handler<List<Models.Books.Historic.BookHistoric>>(resp);
        }
    }
}
