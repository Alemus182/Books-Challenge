using Application.Dtos;
using Application.Interfaces.Infraestructure.Services;
using Application.Services.Books.Responses;
using MediatR;

namespace Application.Services.Books.Queries
{
    public class SearchBooksRequest : IRequest<BookSearchResponse>
    {
        public string Query { get; set; } = string.Empty;

        public class SearchBooksHandler : IRequestHandler<SearchBooksRequest, BookSearchResponse>
        {
            private readonly IOpenLibraryService _openLibraryService;
            private readonly IGeminiService _geminiService;

            public SearchBooksHandler(IOpenLibraryService openLibraryService, IGeminiService geminiService)
            {
                _openLibraryService = openLibraryService;
                _geminiService = geminiService;
            }

            public async Task<BookSearchResponse> Handle(SearchBooksRequest request, CancellationToken cancellationToken)
            {
                var extracted = await _geminiService.ExtractFieldsAsync(request.Query);
                var candidates = await _openLibraryService.SearchBooksAsync(extracted.Title, extracted.Author, extracted.Keywords);

                if (!candidates.Any())
                    return new BookSearchResponse { Candidates = new List<BookCandidateDto>() };

                var ranked = await _geminiService.ExplainAndRerankAsync(request.Query, candidates);
                return new BookSearchResponse { Candidates = ranked.Take(5).ToList() };
            }
        }
    }
}
