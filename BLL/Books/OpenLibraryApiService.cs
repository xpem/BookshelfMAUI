using ApiDAL;
using Models.Books.GoogleApi;
using Models.Books.OpenLibrary;
using System.Text.Json;

namespace Services.Books
{
    /// <summary>
    /// BLL service for searching books via the Open Library API.
    /// Returns <see cref="UIGoogleBook"/> to remain compatible with existing ViewModels and Views.
    /// </summary>
    public static class OpenLibraryApiService
    {
        private const string CoverBaseUrl = "https://covers.openlibrary.org/b/id/{0}-M.jpg";

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        /// <summary>
        /// Search books by title on Open Library.
        /// </summary>
        /// <param name="search">Title to search for.</param>
        /// <param name="page">1-based page number.</param>
        /// <returns>Tuple of (list of books for UI, total number of results).</returns>
        public static async Task<(List<UIGoogleBook>, int)> GetBooks(string search, int page)
        {
            try
            {
                Models.Responses.ApiResponse apiResponse = await OpenLibraryApiDAL.GetBooksByTitleAsync(search, page);

                if (apiResponse.Success && apiResponse.Content is not null)
                    return BuildListBooksResult(apiResponse.Content);

                throw new Exception($"Open Library API error. Content: {apiResponse.Content}");
            }
            catch { throw; }
        }

        private static (List<UIGoogleBook>, int) BuildListBooksResult(string json)
        {
            OpenLibrarySearchResult? result = JsonSerializer.Deserialize<OpenLibrarySearchResult>(json, _jsonOptions)
                ?? throw new Exception("Failed to deserialize Open Library response.");

            List<UIGoogleBook> list = result.Docs
                .Select(MapToUIBook)
                .ToList();

            return (list, result.NumFound);
        }

        private static UIGoogleBook MapToUIBook(OpenLibraryDoc doc)
        {
            // Key comes as "/works/OL27448W" — store only the ID part
            string id = doc.Key?.Replace("/works/", string.Empty) ?? string.Empty;

            string authors = doc.AuthorName is { Count: > 0 }
                ? string.Join("; ", doc.AuthorName)
                : string.Empty;

            string thumbnail = doc.CoverId.HasValue
                ? string.Format(CoverBaseUrl, doc.CoverId.Value)
                : string.Empty;

            string publisher = doc.Publisher is { Count: > 0 }
                ? doc.Publisher[0]
                : string.Empty;

            return new UIGoogleBook
            {
                Id = id,
                Title = doc.Title ?? string.Empty,
                Subtitle = doc.Subtitle ?? string.Empty,
                Authors = authors,
                PublishedDate = doc.FirstPublishYear?.ToString() ?? string.Empty,
                PageCount = doc.NumberOfPagesMedian ?? 0,
                Thumbnail = thumbnail,
                Publisher = publisher,
                Description = string.Empty,
                Categories = string.Empty
            };
        }
    }
}
