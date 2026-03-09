namespace Application.Dtos
{
    public record BookCandidateDto
    {
        public string Title { get; init; } = string.Empty;
        public string Author { get; init; } = string.Empty;
        public int? FirstPublishYear { get; init; }
        public string OpenLibraryId { get; init; } = string.Empty;
        public string? CoverImageUrl { get; init; }
        public string Explanation { get; init; } = string.Empty;
    }
}
