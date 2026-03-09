using Application.Dtos;

namespace Application.Services.Books.Responses
{
    public record BookSearchResponse
    {
        public IList<BookCandidateDto> Candidates { get; init; } = new List<BookCandidateDto>();
    }
}
