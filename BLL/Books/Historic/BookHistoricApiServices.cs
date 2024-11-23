using ApiDAL.Interfaces;
using Models.DTOs;
using Models.Responses;
using Services.Books.Historic.Interfaces;
using Services.Handlers;

namespace Services.Books.Historic
{
    public class BookHistoricApiServices(IBookHistoricApiRepo bookHistoricApiDAL) : IBookHistoricApiServices
    {
        public async Task<BLLResponse> GetByLastCreatedAtAsync(DateTime lastCreatedAt, int page)
        {
            ApiResponse resp = await bookHistoricApiDAL.GetByLastCreatedAtAsync(lastCreatedAt, page);

            return ApiResponseHandler.Handler<List<BookHistoric>>(resp);
        }
    }
}
