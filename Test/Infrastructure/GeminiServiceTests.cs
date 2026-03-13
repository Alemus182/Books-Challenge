using Application.Dtos;
using Application.Interfaces.Infraestructure.Services;
using Application.Models;
using Infraestructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text.Json;

namespace Test.Infrastructure
{
    public class GeminiServiceTests
    {
        private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNamingPolicy = null };

        private static GeminiService CreateService(
            Func<HttpRequestMessage, HttpResponseMessage> handler)
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync((HttpRequestMessage req, CancellationToken _) => handler(req));

            var client = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri("https://generativelanguage.googleapis.com/")
            };

            var factoryMock = new Mock<IHttpClientFactory>();
            factoryMock.Setup(f => f.CreateClient("gemini")).Returns(client);

            var configMock = new Mock<IConfiguration>();
            configMock.Setup(c => c["Gemini:ApiKey"]).Returns("test-api-key");

            var loggerMock = new Mock<ILogger<GeminiService>>();

            var promptProviderMock = new Mock<IPromptProvider>();
            promptProviderMock.Setup(p => p.GetExtractFieldsPrompt()).Returns(new PromptTemplate
            {
                Version = "1.0-test",
                Template = "Extract fields: {0}",
                Config = new GenerationConfig { Temperature = 0.1, TopK = 20, TopP = 0.8, MaxOutputTokens = 512 }
            });
            promptProviderMock.Setup(p => p.GetRerankCandidatesPrompt()).Returns(new PromptTemplate
            {
                Version = "1.0-test",
                Template = "Rerank: {0} {1}",
                Config = new GenerationConfig { Temperature = 0.3, TopK = 40, TopP = 0.9, MaxOutputTokens = 1024 }
            });

            return new GeminiService(factoryMock.Object, configMock.Object, loggerMock.Object, promptProviderMock.Object);
        }

        /// Wraps plain text inside a Gemini API response envelope.
        private static string GeminiEnvelope(string text) =>
            JsonSerializer.Serialize(new
            {
                candidates = new[]
                {
                    new { content = new { parts = new[] { new { text } } } }
                }
            });

        private static HttpResponseMessage Ok(string body) =>
            new(HttpStatusCode.OK) { Content = new StringContent(body) };

        // ─── ExtractFieldsAsync ──────────────────────────────────────────────────

        [Fact]
        public async Task ExtractFieldsAsync_ReturnsExtractedFields_WhenApiReturnsValidJson()
        {
            var json = """{"title": "The Hobbit", "author": "J.R.R. Tolkien", "keywords": ["fantasy", "adventure"]}""";
            var service = CreateService(_ => Ok(GeminiEnvelope(json)));

            var result = await service.ExtractFieldsAsync("tolkien hobbit");

            Assert.Equal("The Hobbit", result.Title);
            Assert.Equal("J.R.R. Tolkien", result.Author);
            Assert.Contains("fantasy", result.Keywords);
            Assert.Contains("adventure", result.Keywords);
        }

        [Fact]
        public async Task ExtractFieldsAsync_ReturnsEmptyDto_WhenApiReturnsInvalidJson()
        {
            var service = CreateService(_ => Ok(GeminiEnvelope("not-valid-json{{")));

            var result = await service.ExtractFieldsAsync("some query");

            Assert.Null(result.Title);
            Assert.Null(result.Author);
            Assert.Empty(result.Keywords);
        }

        [Fact]
        public async Task ExtractFieldsAsync_StripsMarkdownFences_BeforeDeserializing()
        {
            var wrapped = "```json\n{\"title\": \"Dune\", \"author\": \"Frank Herbert\", \"keywords\": []}\n```";
            var service = CreateService(_ => Ok(GeminiEnvelope(wrapped)));

            var result = await service.ExtractFieldsAsync("dune herbert");

            Assert.Equal("Dune", result.Title);
            Assert.Equal("Frank Herbert", result.Author);
        }

        [Fact]
        public async Task ExtractFieldsAsync_ReturnsEmptyKeywords_WhenKeywordsFieldIsNull()
        {
            var json = """{"title": "1984", "author": "George Orwell", "keywords": null}""";
            var service = CreateService(_ => Ok(GeminiEnvelope(json)));

            var result = await service.ExtractFieldsAsync("orwell 1984");

            Assert.Equal("1984", result.Title);
            Assert.NotNull(result.Keywords);
            Assert.Empty(result.Keywords);
        }

        // ─── ExplainAndRerankAsync ───────────────────────────────────────────────

        [Fact]
        public async Task ExplainAndRerankAsync_ReturnsEmptyList_WhenCandidatesIsEmpty()
        {
            var service = CreateService(_ => Ok(GeminiEnvelope("[]")));

            var result = await service.ExplainAndRerankAsync("query", new List<BookCandidateDto>());

            Assert.Empty(result);
        }

        [Fact]
        public async Task ExplainAndRerankAsync_ReranksCandidates_ByApiOrder()
        {
            var candidates = new List<BookCandidateDto>
            {
                new() { OpenLibraryId = "/works/A", Title = "Alpha" },
                new() { OpenLibraryId = "/works/B", Title = "Beta" }
            };
            var rankJson = """[{"openLibraryId": "/works/B", "explanation": "Best match."}, {"openLibraryId": "/works/A", "explanation": "Second."}]""";
            var service = CreateService(_ => Ok(GeminiEnvelope(rankJson)));

            var result = await service.ExplainAndRerankAsync("query", candidates);

            Assert.Equal(2, result.Count);
            Assert.Equal("/works/B", result[0].OpenLibraryId);
            Assert.Equal("Best match.", result[0].Explanation);
            Assert.Equal("/works/A", result[1].OpenLibraryId);
            Assert.Equal("Second.", result[1].Explanation);
        }

        [Fact]
        public async Task ExplainAndRerankAsync_ReturnsOriginalCandidates_WhenApiResponseIsInvalidJson()
        {
            var candidates = new List<BookCandidateDto>
            {
                new() { OpenLibraryId = "/works/A", Title = "Alpha" }
            };
            var service = CreateService(_ => Ok(GeminiEnvelope("bad-json{{")));

            var result = await service.ExplainAndRerankAsync("query", candidates);

            Assert.Single(result);
            Assert.Equal("/works/A", result[0].OpenLibraryId);
        }

        [Fact]
        public async Task ExplainAndRerankAsync_AppendsCandidatesNotReturnedByApi()
        {
            var candidates = new List<BookCandidateDto>
            {
                new() { OpenLibraryId = "/works/A", Title = "Alpha" },
                new() { OpenLibraryId = "/works/B", Title = "Beta" }
            };
            // API only ranks /works/A, omits /works/B
            var rankJson = """[{"openLibraryId": "/works/A", "explanation": "Only this one."}]""";
            var service = CreateService(_ => Ok(GeminiEnvelope(rankJson)));

            var result = await service.ExplainAndRerankAsync("query", candidates);

            Assert.Equal(2, result.Count);
            Assert.Equal("/works/A", result[0].OpenLibraryId);
            Assert.Equal("/works/B", result[1].OpenLibraryId);
        }

        [Fact]
        public void GeminiService_ThrowsInvalidOperationException_WhenApiKeyNotConfigured()
        {
            var factoryMock = new Mock<IHttpClientFactory>();
            var configMock = new Mock<IConfiguration>();
            configMock.Setup(c => c["Gemini:ApiKey"]).Returns((string?)null);
            var loggerMock = new Mock<ILogger<GeminiService>>();
            var promptProviderMock = new Mock<IPromptProvider>();

            Assert.Throws<InvalidOperationException>(() =>
                new GeminiService(factoryMock.Object, configMock.Object, loggerMock.Object, promptProviderMock.Object));
        }
    }
}
