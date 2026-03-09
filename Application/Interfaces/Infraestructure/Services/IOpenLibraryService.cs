using Application.Dtos;

namespace Application.Interfaces.Infraestructure.Services
{
    public interface IOpenLibraryService
    {
        Task<IList<BookCandidateDto>> SearchBooksAsync(string? title, string? author, IList<string> keywords);
    }
}
