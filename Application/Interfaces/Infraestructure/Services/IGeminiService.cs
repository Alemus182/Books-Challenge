using Application.Dtos;

namespace Application.Interfaces.Infraestructure.Services
{
    public interface IGeminiService
    {
        Task<ExtractedQueryDto> ExtractFieldsAsync(string rawQuery);
        Task<IList<BookCandidateDto>> ExplainAndRerankAsync(string rawQuery, IList<BookCandidateDto> candidates);
    }
}
