using Models.Books;
using Models.Responses;
using System.Text.Json;

namespace ApiDAL
{
    public class BookApiDAL
    {
        public static async Task<ApiResponse> AddBook(Book book)
        {
            try
            {
                var json = JsonSerializer.Serialize(book);

                return await HttpClientFunctions.AuthRequest(RequestsTypes.Post, ApiKeys.ApiUri + "/book", json);
            }
            catch (Exception ex) { throw ex; }
        }

        public static async Task<ApiResponse> AltBook(Book book)
        {
            try
            {
                string json = JsonSerializer.Serialize(book);

                return await HttpClientFunctions.AuthRequest(RequestsTypes.Put, ApiKeys.ApiUri + "/Book/" + book.Id, json);
            }
            catch (Exception ex) { throw ex; }
        }

        public static async Task<ApiResponse> GetBooksByLastUpdate(DateTime lastUpdate) =>
            await HttpClientFunctions.AuthRequest(RequestsTypes.Get, ApiKeys.ApiUri + "/book/byupdatedat/" + lastUpdate.ToString("yyyy-MM-ddThh:mm:ss.fff"));

    }
}
