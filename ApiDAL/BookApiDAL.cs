using ApiDAL.Interfaces;
using Models.Books;
using Models.Responses;
using System.Text.Json;

namespace ApiDAL
{
    public class BookApiDAL(IHttpClientFunctions httpClientFunctions) : IBookApiDAL
    {
        public async Task<ApiResponse> AddBook(Book book)
        {
            try
            {
                string json = JsonSerializer.Serialize(book);

                return await httpClientFunctions.AuthRequest(RequestsTypes.Post, ApiKeys.ApiAddress + "/bookshelf/book", json);
            }
            catch (Exception ex) { throw; }
        }

        public async Task<ApiResponse> UpdateBook(Book book)
        {
            try
            {
                string json = JsonSerializer.Serialize(book);

                return await httpClientFunctions.AuthRequest(RequestsTypes.Put, ApiKeys.ApiAddress + "/bookshelf/book/" + book.Id, json);
            }
            catch (Exception ex) { throw; }
        }

        public async Task<ApiResponse> GetBooksByLastUpdate(DateTime lastUpdate) =>
            await httpClientFunctions.AuthRequest(RequestsTypes.Get, ApiKeys.ApiAddress + "/bookshelf/book/byupdatedat/" + lastUpdate.ToString("yyyy-MM-ddThh:mm:ss.fff"));

    }
}
