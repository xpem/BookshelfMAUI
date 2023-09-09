using ApiDAL.Interfaces;
using BLL.Books.Historic.Interfaces;
using BLL.Handlers;
using Models.Responses;

namespace BLL.Books.Historic
{
    public class BookHistoricApiBLL : IBookHistoricApiBLL
    {
        readonly IBookHistoricApiDAL BookHistoricApiDAL;

        public BookHistoricApiBLL(IBookHistoricApiDAL bookHistoricApiDAL) { BookHistoricApiDAL = bookHistoricApiDAL; }

        public async Task<BLLResponse> GetBookHistoricByLastCreatedAt(DateTime lastCreatedAt)
        {
            ApiResponse resp = await BookHistoricApiDAL.GetBooksHistoricByLastCreatedAt(lastCreatedAt);

            return ApiResponseHandler.Handler<List<Models.Books.Historic.BookHistoric>>(resp);
        }
    }
}
