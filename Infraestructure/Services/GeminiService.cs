using Application.Dtos;
using Application.Interfaces.Infraestructure.Services;
using Application.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace Infraestructure.Services
{
    public class GeminiService : IGeminiService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IPromptProvider _promptProvider;
        private readonly ILogger<GeminiService> _logger;
        private readonly string _apiKey;
        private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

        public GeminiService(
            IHttpClientFactory httpClientFactory, 
            IConfiguration configuration, 
            ILogger<GeminiService> logger,
            IPromptProvider promptProvider)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _promptProvider = promptProvider;
            _apiKey = configuration["Gemini:ApiKey"]
                      ?? throw new InvalidOperationException("Gemini:ApiKey is not configured.");
        }

        public async Task<ExtractedQueryDto> ExtractFieldsAsync(string rawQuery)
        {
            var promptTemplate = _promptProvider.GetExtractFieldsPrompt();
            var prompt = string.Format(promptTemplate.Template, rawQuery);

            _logger.LogInformation("ExtractFields using prompt version {Version}", promptTemplate.Version);

            var text = await CallGeminiAsync(prompt, promptTemplate.Config);
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
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to deserialize extracted fields from Gemini response");
                return new ExtractedQueryDto { Keywords = new List<string>() };
            }
        }

        public async Task<IList<BookCandidateDto>> ExplainAndRerankAsync(string rawQuery, IList<BookCandidateDto> candidates)
        {
            if (!candidates.Any()) return candidates;

            var summary = JsonSerializer.Serialize(
                candidates.Select(c => new { c.OpenLibraryId, c.Title, c.Author, c.FirstPublishYear }),
                JsonOptions);

            var promptTemplate = _promptProvider.GetRerankCandidatesPrompt();
            var prompt = string.Format(promptTemplate.Template, rawQuery, summary);

            _logger.LogInformation("Reranking using prompt version {Version}", promptTemplate.Version);

            var text = await CallGeminiAsync(prompt, promptTemplate.Config);
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
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to deserialize reranked results from Gemini response");
                return candidates;
            }
        }

        private async Task<string> CallGeminiAsync(string prompt, GenerationConfig? config = null)
        {
            var client = _httpClientFactory.CreateClient("gemini");

            var request = new GeminiRequest
            {
                Contents = new List<GeminiContent>
                {
                    new() { Parts = new List<GeminiPart> { new() { Text = prompt } } }
                },
                GenerationConfig = config != null ? new GeminiGenerationConfig
                {
                    Temperature = config.Temperature,
                    TopK = config.TopK,
                    TopP = config.TopP,
                    MaxOutputTokens = config.MaxOutputTokens
                } : null
            };

            var requestJson = JsonSerializer.Serialize(request, JsonOptions);
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
}
