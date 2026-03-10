using Application.Dtos;
using Application.Interfaces.Infraestructure.Services;
using Application.Models;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Infraestructure.Services
{
    public class OpenLibraryService : IOpenLibraryService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

        public OpenLibraryService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IList<BookCandidateDto>> SearchBooksAsync(string? title, string? author, IList<string> keywords)
        {
            var client = _httpClientFactory.CreateClient("openlibrary");

            var queryParts = new List<string>();
            if (!string.IsNullOrWhiteSpace(title))
                queryParts.Add($"title={Uri.EscapeDataString(title)}");
            if (!string.IsNullOrWhiteSpace(author))
                queryParts.Add($"author={Uri.EscapeDataString(author)}");
            if (queryParts.Count == 0 && keywords.Any())
                queryParts.Add($"q={Uri.EscapeDataString(string.Join(" ", keywords))}");
            else if (queryParts.Count == 0)
                return new List<BookCandidateDto>();

            queryParts.Add("fields=key,title,author_name,first_publish_year,cover_i");
            queryParts.Add("limit=10");

            var response = await client.GetAsync($"search.json?{string.Join("&", queryParts)}");
            if (!response.IsSuccessStatusCode)
                return new List<BookCandidateDto>();

            var json = await response.Content.ReadAsStringAsync();
            var searchResult = JsonSerializer.Deserialize<OlSearchResult>(json, JsonOptions);
            if (searchResult?.Docs == null || !searchResult.Docs.Any())
                return new List<BookCandidateDto>();

            var candidates = new List<(BookCandidateDto Candidate, int Score)>();
            var seen = new HashSet<string>();

            foreach (var doc in searchResult.Docs.Take(5))
            {
                if (string.IsNullOrEmpty(doc.Key) || seen.Contains(doc.Key)) continue;
                seen.Add(doc.Key);

                var primaryAuthor = await ResolvePrimaryAuthorAsync(client, doc.Key, doc.AuthorName);
                var coverUrl = doc.CoverId.HasValue
                    ? $"https://covers.openlibrary.org/b/id/{doc.CoverId}-L.jpg"
                    : null;

                var (explanation, score) = BuildExplanation(doc.Title, doc.AuthorName, title, author, primaryAuthor);
                candidates.Add((new BookCandidateDto
                {
                    Title = doc.Title ?? string.Empty,
                    Author = primaryAuthor ?? doc.AuthorName?.FirstOrDefault() ?? string.Empty,
                    FirstPublishYear = doc.FirstPublishYear,
                    OpenLibraryId = doc.Key,
                    CoverImageUrl = coverUrl,
                    Explanation = explanation
                }, score));
            }

            return candidates
                .OrderBy(c => c.Score)
                .Select(c => c.Candidate)
                .ToList();
        }

        private static (string explanation, int score) BuildExplanation(
            string? docTitle, IList<string>? authorNames,
            string? queryTitle, string? queryAuthor, string? primaryAuthor)
        {
            var normDocTitle = Normalize(docTitle ?? string.Empty);
            var normQueryTitle = Normalize(queryTitle ?? string.Empty);
            var normQueryAuthor = Normalize(queryAuthor ?? string.Empty);
            var normPrimary = Normalize(primaryAuthor ?? string.Empty);

            bool exactTitle = !string.IsNullOrEmpty(normQueryTitle) && normDocTitle == normQueryTitle;
            bool nearTitle = !exactTitle && !string.IsNullOrEmpty(normQueryTitle) && normDocTitle.Contains(normQueryTitle);
            bool primaryMatch = !string.IsNullOrEmpty(normQueryAuthor) && !string.IsNullOrEmpty(normPrimary)
                                && normPrimary.Contains(normQueryAuthor);
            bool contributorMatch = !primaryMatch && !string.IsNullOrEmpty(normQueryAuthor)
                                    && (authorNames?.Any(a => Normalize(a).Contains(normQueryAuthor)) ?? false);

            if (exactTitle && primaryMatch)
                return ($"Exact title match; {primaryAuthor} is primary author.", 1);
            if (exactTitle && contributorMatch)
                return ("Exact title match; author listed as contributor.", 2);
            if (exactTitle)
                return ("Exact title match.", 2);
            if (nearTitle && primaryMatch)
                return ($"Near title match; {primaryAuthor} is primary author.", 3);
            if (primaryMatch)
                return ($"Author match; {primaryAuthor} is primary author.", 4);
            return ("Candidate match based on search relevance.", 5);
        }

        private async Task<string?> ResolvePrimaryAuthorAsync(HttpClient client, string workKey, IList<string>? fallback)
        {
            try
            {
                var workResponse = await client.GetAsync($"{workKey.TrimStart('/')}.json");
                if (!workResponse.IsSuccessStatusCode) return fallback?.FirstOrDefault();

                var workJson = await workResponse.Content.ReadAsStringAsync();
                var work = JsonSerializer.Deserialize<OlWork>(workJson, JsonOptions);
                var authorKey = work?.Authors?.FirstOrDefault()?.Author?.Key;
                if (string.IsNullOrEmpty(authorKey)) return fallback?.FirstOrDefault();

                var authorResponse = await client.GetAsync($"{authorKey.TrimStart('/')}.json");
                if (!authorResponse.IsSuccessStatusCode) return fallback?.FirstOrDefault();

                var authorJson = await authorResponse.Content.ReadAsStringAsync();
                var authorDetail = JsonSerializer.Deserialize<OlAuthor>(authorJson, JsonOptions);
                return authorDetail?.Name ?? fallback?.FirstOrDefault();
            }
            catch
            {
                return fallback?.FirstOrDefault();
            }
        }

        private static string Normalize(string input) =>
            Regex.Replace(input.ToLowerInvariant(), @"[^\w\s]", " ").Trim();
    }
}
