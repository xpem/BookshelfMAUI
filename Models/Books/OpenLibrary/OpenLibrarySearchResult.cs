using System.Text.Json.Serialization;

namespace Models.Books.OpenLibrary
{
    /// <summary>
    /// Root response from https://openlibrary.org/search.json
    /// </summary>
    public class OpenLibrarySearchResult
    {
        [JsonPropertyName("num_found")]
        public int NumFound { get; set; }

        [JsonPropertyName("start")]
        public int Start { get; set; }

        [JsonPropertyName("docs")]
        public List<OpenLibraryDoc> Docs { get; set; } = [];
    }

    /// <summary>
    /// A single document entry returned by the Open Library Search API.
    /// </summary>
    public class OpenLibraryDoc
    {
        /// <summary>
        /// Work key, e.g. "/works/OL27448W". Strip the "/works/" prefix for use as ID.
        /// </summary>
        [JsonPropertyName("key")]
        public string Key { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("subtitle")]
        public string? Subtitle { get; set; }

        [JsonPropertyName("author_name")]
        public List<string>? AuthorName { get; set; }

        [JsonPropertyName("first_publish_year")]
        public int? FirstPublishYear { get; set; }

        /// <summary>
        /// Internal cover ID — use with https://covers.openlibrary.org/b/id/{cover_i}-S.jpg
        /// </summary>
        [JsonPropertyName("cover_i")]
        public int? CoverId { get; set; }

        [JsonPropertyName("number_of_pages_median")]
        public int? NumberOfPagesMedian { get; set; }

        [JsonPropertyName("publisher")]
        public List<string>? Publisher { get; set; }

        [JsonPropertyName("subject")]
        public List<string>? Subject { get; set; }

        [JsonPropertyName("isbn")]
        public List<string>? Isbn { get; set; }

        [JsonPropertyName("editions")]
        public OpenLibraryEditions? Editions { get; set; }
    }

    /// <summary>
    /// Container for edition documents nested inside a work.
    /// </summary>
    public class OpenLibraryEditions
    {
        [JsonPropertyName("docs")]
        public List<OpenLibraryEditionDoc>? Docs { get; set; }
    }

    /// <summary>
    /// A single edition entry (localized title, subtitle, cover).
    /// </summary>
    public class OpenLibraryEditionDoc
    {
        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("subtitle")]
        public string? Subtitle { get; set; }

        [JsonPropertyName("cover_i")]
        public int? CoverId { get; set; }
    }
}
