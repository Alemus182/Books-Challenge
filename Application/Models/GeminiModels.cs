using System.Text.Json.Serialization;

namespace Application.Models
{
    public record GeminiRequest
    {
        [JsonPropertyName("contents")]
        public IList<GeminiContent> Contents { get; init; } = new List<GeminiContent>();

        [JsonPropertyName("generationConfig")]
        public GeminiGenerationConfig? GenerationConfig { get; init; }
    }

    public record GeminiContent
    {
        [JsonPropertyName("parts")]
        public IList<GeminiPart> Parts { get; init; } = new List<GeminiPart>();
    }

    public record GeminiPart
    {
        [JsonPropertyName("text")]
        public string Text { get; init; } = string.Empty;
    }

    public record GeminiGenerationConfig
    {
        [JsonPropertyName("temperature")]
        public double? Temperature { get; init; }

        [JsonPropertyName("topK")]
        public int? TopK { get; init; }

        [JsonPropertyName("topP")]
        public double? TopP { get; init; }

        [JsonPropertyName("maxOutputTokens")]
        public int? MaxOutputTokens { get; init; }
    }

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
