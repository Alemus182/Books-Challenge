namespace Application.Models
{
    public class PromptTemplate
    {
        public string Version { get; init; } = "1.0";
        public string Template { get; init; } = string.Empty;
        public GenerationConfig Config { get; init; } = new();
    }

    public class GenerationConfig
    {
        public double Temperature { get; init; } = 0.2;
        public int TopK { get; init; } = 40;
        public double TopP { get; init; } = 0.95;
        public int MaxOutputTokens { get; init; } = 2048;
    }
}
