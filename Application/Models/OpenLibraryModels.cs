using System.Text.Json.Serialization;

namespace Application.Models
{
    public record OlSearchResult
    {
        [JsonPropertyName("docs")]
        public IList<OlDoc>? Docs { get; init; }
    }

    public record OlDoc
    {
        [JsonPropertyName("key")]
        public string? Key { get; init; }

        [JsonPropertyName("title")]
        public string? Title { get; init; }

        [JsonPropertyName("author_name")]
        public IList<string>? AuthorName { get; init; }

        [JsonPropertyName("first_publish_year")]
        public int? FirstPublishYear { get; init; }

        [JsonPropertyName("cover_i")]
        public int? CoverId { get; init; }
    }

    public record OlWork
    {
        [JsonPropertyName("authors")]
        public IList<OlWorkAuthor>? Authors { get; init; }
    }

    public record OlWorkAuthor
    {
        [JsonPropertyName("author")]
        public OlRef? Author { get; init; }
    }

    public record OlRef
    {
        [JsonPropertyName("key")]
        public string? Key { get; init; }
    }

    public record OlAuthor
    {
        [JsonPropertyName("name")]
        public string? Name { get; init; }
    }
}
