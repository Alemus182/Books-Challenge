namespace Application.Dtos
{
    public record ExtractedQueryDto
    {
        public string? Title { get; init; }
        public string? Author { get; init; }
        public IList<string> Keywords { get; init; } = new List<string>();
    }
}
