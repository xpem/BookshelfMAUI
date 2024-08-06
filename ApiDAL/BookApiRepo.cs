using ApiDAL.Interfaces;
using Models.Books;
using Models.Responses;
using System.Text.Json;

namespace ApiDAL
{
    public class BookApiRepo(IHttpClientFunctions httpClientFunctions) : IBookApiRepo
    {
        public async Task<ApiResponse> CreateAsync(Book book)
        {
            try
            {
                string json = JsonSerializer.Serialize(book);

                return await httpClientFunctions.AuthRequest(RequestsTypes.Post, ApiKeys.ApiAddress + "/bookshelf/book", json);
            }
            catch (Exception ex) { throw; }
        }

        public async Task<ApiResponse> UpdateAsync(Book book)
        {
            try
            {
                string json = JsonSerializer.Serialize(book);

                return await httpClientFunctions.AuthRequest(RequestsTypes.Put, ApiKeys.ApiAddress + "/bookshelf/book/" + book.Id, json);
            }
            catch (Exception ex) { throw; }
        }

        public async Task<ApiResponse> GetByLastUpdateAsync(DateTime lastUpdate, int page) =>
            await httpClientFunctions.AuthRequest(RequestsTypes.Get, $"{ApiKeys.ApiAddress}/bookshelf/book/byupdatedat/{lastUpdate:yyyy-MM-ddThh:mm:ss.fff}/{page}");

    }
}
