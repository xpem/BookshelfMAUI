using ApiDAL;
using BLL.Handlers;
using Models.Responses;

namespace BLL.Books.Historic
{
    public class BookHistoricBLL
    {
        public static async Task<BLLResponse> GetBookHistoricByLastCreatedAt(DateTime lastCreatedAt)
        {
            var resp = await BookHistoricApiDAL.GetBooksHistoricByLastCreatedAt(lastCreatedAt);

            return ApiResponseHandler.Handler<List<Models.Books.Historic.BookHistoric>>(resp);
        }
    }
}
