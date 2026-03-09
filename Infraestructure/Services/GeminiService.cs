using Application.Dtos;
using Application.Interfaces.Infraestructure.Services;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Infraestructure.Services
{
    public class GeminiService : IGeminiService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiKey;
        private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

        public GeminiService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _apiKey = configuration["Gemini:ApiKey"]
                      ?? throw new InvalidOperationException("Gemini:ApiKey is not configured.");
        }

        public async Task<ExtractedQueryDto> ExtractFieldsAsync(string rawQuery)
        {
            var prompt = $$"""
                Extract structured fields from this book search query.
                Return ONLY valid JSON with this exact shape (use null for missing fields, no markdown):
                {"title": "...", "author": "...", "keywords": ["..."]}

                Query: {{rawQuery}}
                """;

            var text = await CallGeminiAsync(prompt);
            try
            {
                var cleaned = StripMarkdown(text);
                var fields = JsonSerializer.Deserialize<GeminiFieldsResult>(cleaned, JsonOptions);
                return new ExtractedQueryDto
                {
                    Title = fields?.Title,
                    Author = fields?.Author,
                    Keywords = fields?.Keywords ?? new List<string>()
                };
            }
            catch
            {
                return new ExtractedQueryDto { Keywords = new List<string>() };
            }
        }

        public async Task<IList<BookCandidateDto>> ExplainAndRerankAsync(string rawQuery, IList<BookCandidateDto> candidates)
        {
            if (!candidates.Any()) return candidates;

            var summary = JsonSerializer.Serialize(
                candidates.Select(c => new { c.OpenLibraryId, c.Title, c.Author, c.FirstPublishYear }),
                JsonOptions);

            var prompt = $$"""
                Rerank these book candidates best-first for the query, and write a one-sentence explanation for each.
                Return ONLY a valid JSON array (no markdown):
                [{"openLibraryId": "...", "explanation": "one sentence"}]

                Query: {{rawQuery}}
                Candidates: {{summary}}
                """;

            var text = await CallGeminiAsync(prompt);
            try
            {
                var cleaned = StripMarkdown(text);
                var rankings = JsonSerializer.Deserialize<IList<GeminiRankItem>>(cleaned, JsonOptions);
                if (rankings == null || !rankings.Any()) return candidates;

                var result = new List<BookCandidateDto>();
                foreach (var rank in rankings)
                {
                    var match = candidates.FirstOrDefault(c => c.OpenLibraryId == rank.OpenLibraryId);
                    if (match != null)
                        result.Add(match with { Explanation = rank.Explanation ?? match.Explanation });
                }
                foreach (var c in candidates.Where(c => result.All(r => r.OpenLibraryId != c.OpenLibraryId)))
                    result.Add(c);

                return result;
            }
            catch
            {
                return candidates;
            }
        }

        private async Task<string> CallGeminiAsync(string prompt)
        {
            var client = _httpClientFactory.CreateClient("gemini");
            var body = new
            {
                contents = new[]
                {
                    new { parts = new[] { new { text = prompt } } }
                }
            };

            var requestJson = JsonSerializer.Serialize(body);
            var content = new StringContent(requestJson, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(
                $"v1beta/models/gemini-2.0-flash:generateContent?key={_apiKey}", content);

            response.EnsureSuccessStatusCode();
            var responseJson = await response.Content.ReadAsStringAsync();
            var geminiResponse = JsonSerializer.Deserialize<GeminiApiResponse>(responseJson, JsonOptions);
            return geminiResponse?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text
                   ?? string.Empty;
        }

        private static string StripMarkdown(string input)
        {
            var trimmed = input.Trim();
            if (!trimmed.StartsWith("```")) return trimmed;
            var start = trimmed.IndexOf('\n') + 1;
            var end = trimmed.LastIndexOf("```");
            return end > start ? trimmed[start..end].Trim() : trimmed;
        }
    }

    internal record GeminiFieldsResult
    {
        [JsonPropertyName("title")]
        public string? Title { get; init; }

        [JsonPropertyName("author")]
        public string? Author { get; init; }

        [JsonPropertyName("keywords")]
        public IList<string>? Keywords { get; init; }
    }

    internal record GeminiRankItem
    {
        [JsonPropertyName("openLibraryId")]
        public string? OpenLibraryId { get; init; }

        [JsonPropertyName("explanation")]
        public string? Explanation { get; init; }
    }

    internal record GeminiApiResponse
    {
        [JsonPropertyName("candidates")]
        public IList<GeminiApiCandidate>? Candidates { get; init; }
    }

    internal record GeminiApiCandidate
    {
        [JsonPropertyName("content")]
        public GeminiApiContent? Content { get; init; }
    }

    internal record GeminiApiContent
    {
        [JsonPropertyName("parts")]
        public IList<GeminiApiPart>? Parts { get; init; }
    }

    internal record GeminiApiPart
    {
        [JsonPropertyName("text")]
        public string? Text { get; init; }
    }
}
