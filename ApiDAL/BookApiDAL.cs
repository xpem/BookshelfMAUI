using ApiDAL.Interfaces;
using Models.Books;
using Models.Responses;
using System.Text.Json;

namespace ApiDAL
{
    public class BookApiDAL: IBookApiDAL
    {

        readonly IHttpClientFunctions HttpClientFunctions;

        public BookApiDAL(IHttpClientFunctions httpClientFunctions) { HttpClientFunctions = httpClientFunctions; }


        public async Task<ApiResponse> AddBook(Book book)
        {
            try
            {
                var json = JsonSerializer.Serialize(book);

                return await HttpClientFunctions.AuthRequest(RequestsTypes.Post, ApiKeys.ApiAddress + "/book", json);
            }
            catch (Exception ex) { throw ex; }
        }

        public async Task<ApiResponse> UpdateBook(Book book)
        {
            try
            {
                string json = JsonSerializer.Serialize(book);

                return await HttpClientFunctions.AuthRequest(RequestsTypes.Put, ApiKeys.ApiAddress + "/Book/" + book.Id, json);
            }
            catch (Exception ex) { throw ex; }
        }

        public async Task<ApiResponse> GetBooksByLastUpdate(DateTime lastUpdate) =>
            await HttpClientFunctions.AuthRequest(RequestsTypes.Get, ApiKeys.ApiAddress + "/book/byupdatedat/" + lastUpdate.ToString("yyyy-MM-ddThh:mm:ss.fff"));

    }
}
