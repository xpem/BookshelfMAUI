using ApiDAL.Interfaces;
using Models.DTOs;
using Models.Responses;
using System.Text.Json;

namespace ApiDAL
{
    public class BookApiRepo(IHttpClientFunctions httpClientFunctions) : IBookApiRepo
    {
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public async Task<ApiResponse> CreateAsync(Book book)
        {
            try
            {
                var req = BuildRequest(book);
                string json = JsonSerializer.Serialize(req);

                return await httpClientFunctions.AuthRequestAsync(RequestsTypes.Post, ApiKeys.ApiAddress + "/bookshelf/book", json);
            }
            catch { throw; }
        }

        public async Task<ApiResponse> UpdateAsync(Book book)
        {
            try
            {
                var req = BuildRequest(book);
                string json = JsonSerializer.Serialize(req);

                return await httpClientFunctions.AuthRequestAsync(RequestsTypes.Put, ApiKeys.ApiAddress + "/bookshelf/book/" + book.Id, json);
            }
            catch { throw; }
        }

        public async Task<ApiResponse> GetByLastUpdateAsync(DateTime lastUpdate, int page) =>
            await httpClientFunctions.AuthRequestAsync(RequestsTypes.Get, $"{ApiKeys.ApiAddress}/bookshelf/book/byupdatedat/{lastUpdate:yyyy-MM-ddTHH:mm:ss.fff}/{page}");

        private static object BuildRequest(Book book) => new
        {
            BookId = (!book.BookId.HasValue || book.BookId == Guid.Empty) ? (Guid?)null : book.BookId,
            book.Cover,
            book.Title,
            Subtitle = book.SubTitle,
            book.Authors,
            book.Volume,
            book.Pages,
            book.Year,
            Status = (int)(book.Status ?? Models.DTOs.Status.None),
            book.Score,
            book.Comment,
            book.Genre,
            book.Isbn,
            book.GoogleId,
        };
    }
}
