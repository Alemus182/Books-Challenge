using System.Text.Json.Serialization;

namespace Application.Models
{
    public record GeminiFieldsResult
    {
        [JsonPropertyName("title")]
        public string? Title { get; init; }

        [JsonPropertyName("author")]
        public string? Author { get; init; }

        [JsonPropertyName("keywords")]
        public IList<string>? Keywords { get; init; }
    }

    public record GeminiRankItem
    {
        [JsonPropertyName("openLibraryId")]
        public string? OpenLibraryId { get; init; }

        [JsonPropertyName("explanation")]
        public string? Explanation { get; init; }
    }

    public record GeminiApiResponse
    {
        [JsonPropertyName("candidates")]
        public IList<GeminiApiCandidate>? Candidates { get; init; }
    }

    public record GeminiApiCandidate
    {
        [JsonPropertyName("content")]
        public GeminiApiContent? Content { get; init; }
    }

    public record GeminiApiContent
    {
        [JsonPropertyName("parts")]
        public IList<GeminiApiPart>? Parts { get; init; }
    }

    public record GeminiApiPart
    {
        [JsonPropertyName("text")]
        public string? Text { get; init; }
    }
}
