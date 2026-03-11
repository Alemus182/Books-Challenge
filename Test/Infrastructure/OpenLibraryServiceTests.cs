using Infraestructure.Services;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text.Json;

namespace Test.Infrastructure
{
    public class OpenLibraryServiceTests
    {
        private static OpenLibraryService CreateService(
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
                BaseAddress = new Uri("https://openlibrary.org/")
            };

            var factoryMock = new Mock<IHttpClientFactory>();
            factoryMock.Setup(f => f.CreateClient("openlibrary")).Returns(client);

            return new OpenLibraryService(factoryMock.Object);
        }

        private static HttpResponseMessage Ok(object body) =>
            new(HttpStatusCode.OK) { Content = new StringContent(JsonSerializer.Serialize(body)) };

        // ─── Edge cases ──────────────────────────────────────────────────────────

        [Fact]
        public async Task SearchBooksAsync_ReturnsEmpty_WhenNoTitleNoAuthorNoKeywords()
        {
            var service = CreateService(_ => new HttpResponseMessage(HttpStatusCode.OK));

            var result = await service.SearchBooksAsync(null, null, new List<string>());

            Assert.Empty(result);
        }

        [Fact]
        public async Task SearchBooksAsync_ReturnsEmpty_WhenApiReturnsNonSuccess()
        {
            var service = CreateService(_ => new HttpResponseMessage(HttpStatusCode.ServiceUnavailable));

            var result = await service.SearchBooksAsync("The Hobbit", null, new List<string>());

            Assert.Empty(result);
        }

        [Fact]
        public async Task SearchBooksAsync_ReturnsEmpty_WhenApiReturnsNoDocs()
        {
            var service = CreateService(req =>
                req.RequestUri!.PathAndQuery.Contains("search.json")
                    ? Ok(new { docs = Array.Empty<object>() })
                    : new HttpResponseMessage(HttpStatusCode.NotFound));

            var result = await service.SearchBooksAsync("NonExistentTitle", null, new List<string>());

            Assert.Empty(result);
        }

        // ─── Happy paths ─────────────────────────────────────────────────────────

        [Fact]
        public async Task SearchBooksAsync_ReturnsCandidates_WhenApiReturnsFullData()
        {
            var searchPayload = new
            {
                docs = new[]
                {
                    new
                    {
                        key = "/works/OL262758W",
                        title = "The Hobbit",
                        author_name = new[] { "J.R.R. Tolkien" },
                        first_publish_year = 1937,
                        cover_i = 8406786
                    }
                }
            };
            var workPayload = new
            {
                authors = new[] { new { author = new { key = "/authors/OL26320A" } } }
            };
            var authorPayload = new { name = "J.R.R. Tolkien" };

            var service = CreateService(req =>
            {
                var url = req.RequestUri!.ToString();
                if (url.Contains("search.json")) return Ok(searchPayload);
                if (url.Contains("works/OL262758W.json")) return Ok(workPayload);
                if (url.Contains("authors/OL26320A.json")) return Ok(authorPayload);
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            });

            var result = await service.SearchBooksAsync("The Hobbit", "Tolkien", new List<string>());

            Assert.Single(result);
            Assert.Equal("The Hobbit", result[0].Title);
            Assert.Equal("J.R.R. Tolkien", result[0].Author);
            Assert.Equal(1937, result[0].FirstPublishYear);
            Assert.Equal("/works/OL262758W", result[0].OpenLibraryId);
            Assert.NotNull(result[0].CoverImageUrl);
            Assert.Contains("8406786", result[0].CoverImageUrl);
        }

        [Fact]
        public async Task SearchBooksAsync_UsesFallbackAuthor_WhenWorkResolutionFails()
        {
            var searchPayload = new
            {
                docs = new[]
                {
                    new { key = "/works/OL1W", title = "Test Book", author_name = new[] { "Fallback Author" }, first_publish_year = 2000, cover_i = (int?)null }
                }
            };

            var service = CreateService(req =>
            {
                var url = req.RequestUri!.ToString();
                if (url.Contains("search.json")) return Ok(searchPayload);
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            });

            var result = await service.SearchBooksAsync("Test Book", null, new List<string>());

            Assert.Single(result);
            Assert.Equal("Fallback Author", result[0].Author);
        }

        [Fact]
        public async Task SearchBooksAsync_NullCoverImageUrl_WhenNoCoverIdReturned()
        {
            var searchPayload = new
            {
                docs = new[]
                {
                    new { key = "/works/OL3W", title = "No Cover Book", author_name = new[] { "Some Author" }, first_publish_year = 2010, cover_i = (int?)null }
                }
            };

            var service = CreateService(req =>
            {
                var url = req.RequestUri!.ToString();
                if (url.Contains("search.json")) return Ok(searchPayload);
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            });

            var result = await service.SearchBooksAsync("No Cover Book", null, new List<string>());

            Assert.Single(result);
            Assert.Null(result[0].CoverImageUrl);
        }

        // ─── Query building ───────────────────────────────────────────────────────

        [Fact]
        public async Task SearchBooksAsync_UsesKeywordsQuery_WhenNoTitleOrAuthorProvided()
        {
            var searchPayload = new
            {
                docs = new[]
                {
                    new { key = "/works/OL4W", title = "Keyword Result", author_name = new[] { "Someone" }, first_publish_year = 2021, cover_i = (int?)null }
                }
            };

            HttpRequestMessage? capturedRequest = null;
            var service = CreateService(req =>
            {
                var url = req.RequestUri!.ToString();
                if (url.Contains("search.json"))
                {
                    capturedRequest = req;
                    return Ok(searchPayload);
                }
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            });

            await service.SearchBooksAsync(null, null, new List<string> { "fantasy", "adventure" });

            Assert.NotNull(capturedRequest);
            Assert.Contains("q=", capturedRequest!.RequestUri!.Query);
            Assert.Contains("fantasy", Uri.UnescapeDataString(capturedRequest.RequestUri.Query));
        }

        [Fact]
        public async Task SearchBooksAsync_IncludesTitleInQuery_WhenTitleProvided()
        {
            var searchPayload = new
            {
                docs = new[]
                {
                    new { key = "/works/OL5W", title = "My Title", author_name = new[] { "Author" }, first_publish_year = 2000, cover_i = (int?)null }
                }
            };

            HttpRequestMessage? capturedRequest = null;
            var service = CreateService(req =>
            {
                var url = req.RequestUri!.ToString();
                if (url.Contains("search.json"))
                {
                    capturedRequest = req;
                    return Ok(searchPayload);
                }
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            });

            await service.SearchBooksAsync("My Title", null, new List<string>());

            Assert.NotNull(capturedRequest);
            Assert.Contains("title=", capturedRequest!.RequestUri!.Query);
        }

        // ─── Explanation / scoring ────────────────────────────────────────────────

        [Fact]
        public async Task SearchBooksAsync_ExactTitleAndAuthorMatch_HasHighestPriority()
        {
            var searchPayload = new
            {
                docs = new[]
                {
                    new { key = "/works/OL6W", title = "Dune", author_name = new[] { "Frank Herbert" }, first_publish_year = 1965, cover_i = (int?)null },
                    new { key = "/works/OL7W", title = "Dune Messiah", author_name = new[] { "Frank Herbert" }, first_publish_year = 1969, cover_i = (int?)null }
                }
            };
            var workPayload1 = new { authors = new[] { new { author = new { key = "/authors/OL27A" } } } };
            var workPayload2 = new { authors = new[] { new { author = new { key = "/authors/OL28A" } } } };
            var authorPayload = new { name = "Frank Herbert" };

            var service = CreateService(req =>
            {
                var url = req.RequestUri!.ToString();
                if (url.Contains("search.json")) return Ok(searchPayload);
                if (url.Contains("works/OL6W.json")) return Ok(workPayload1);
                if (url.Contains("works/OL7W.json")) return Ok(workPayload2);
                if (url.Contains("authors/")) return Ok(authorPayload);
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            });

            var result = await service.SearchBooksAsync("Dune", "Frank Herbert", new List<string>());

            // Exact title + primary author match should be ranked first
            Assert.Equal("Dune", result[0].Title);
        }
    }
}
