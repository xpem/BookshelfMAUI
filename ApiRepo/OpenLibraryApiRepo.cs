using Models.Responses;
using System.Net;
using System.Text;

namespace ApiRepo
{
    /// <summary>
    /// Data access layer for the Open Library Search API.
    /// Docs: https://openlibrary.org/dev/docs/api/search
    /// </summary>
    public class OpenLibraryApiRepo
    {
        private const string BaseUrl = "https://openlibrary.org/search.json";

        /// <summary>
        /// Fields requested from the API — keeps response payload small.
        /// </summary>
        private const string Fields = "key,title,subtitle,author_name,first_publish_year,cover_i,number_of_pages_median,publisher,isbn";

        private static readonly HttpClient _httpClient = new();

        static OpenLibraryApiRepo()
        {
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation(
                "User-Agent", "BookshelfApp (contact@bookshelfapp.com)");
        }

        /// <summary>
        /// Search books by title on Open Library.
        /// </summary>
        /// <param name="search">Title query string.</param>
        /// <param name="page">1-based page number.</param>
        /// <param name="limit">Results per page (default 10).</param>
        public static async Task<ApiResponse> GetBooksByTitleAsync(string search, int page, int limit = 10)
        {
            StringBuilder url = new();
            url.Append($"{BaseUrl}?title={Uri.EscapeDataString(search)}");
            url.Append($"&fields={Fields}");
            url.Append($"&page={page}");
            url.Append($"&limit={limit}");
            url.Append($"&lang=pt");

            try
            {
                HttpResponseMessage httpResponse = await _httpClient.GetAsync(url.ToString());

                return new ApiResponse()
                {
                    Success = httpResponse.IsSuccessStatusCode,
                    Error = httpResponse.StatusCode == HttpStatusCode.Unauthorized ? ErrorTypes.Unauthorized : null,
                    Content = await httpResponse.Content.ReadAsStringAsync()
                };
            }
            catch { throw; }
        }
    }
}
