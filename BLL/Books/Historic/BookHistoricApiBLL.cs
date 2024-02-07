using ApiDAL.Interfaces;
using BLL.Books.Historic.Interfaces;
using BLL.Handlers;
using Models.Responses;

namespace BLL.Books.Historic
{
    public class BookHistoricApiBLL(IBookHistoricApiDAL bookHistoricApiDAL) : IBookHistoricApiBLL
    {
        public async Task<BLLResponse> GetBookHistoricByLastCreatedAt(DateTime lastCreatedAt)
        {
            ApiResponse resp = await bookHistoricApiDAL.GetBooksHistoricByLastCreatedAt(lastCreatedAt);

            return ApiResponseHandler.Handler<List<Models.Books.Historic.BookHistoric>>(resp);
        }
    }
}
