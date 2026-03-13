using System.Text.Json.Serialization;

namespace Application.Models
{
    public class PromptConfigurationFile
    {
        [JsonPropertyName("extractFields")]
        public PromptDefinition? ExtractFields { get; init; }

        [JsonPropertyName("rerankCandidates")]
        public PromptDefinition? RerankCandidates { get; init; }

        [JsonPropertyName("versionHistory")]
        public Dictionary<string, string>? VersionHistory { get; init; }
    }

    public class PromptDefinition
    {
        [JsonPropertyName("version")]
        public string Version { get; init; } = "1.0";

        [JsonPropertyName("template")]
        public string Template { get; init; } = string.Empty;

        [JsonPropertyName("config")]
        public GenerationConfigJson? Config { get; init; }

        [JsonPropertyName("description")]
        public string? Description { get; init; }
    }

    public class GenerationConfigJson
    {
        [JsonPropertyName("temperature")]
        public double Temperature { get; init; } = 0.2;

        [JsonPropertyName("topK")]
        public int TopK { get; init; } = 40;

        [JsonPropertyName("topP")]
        public double TopP { get; init; } = 0.95;

        [JsonPropertyName("maxOutputTokens")]
        public int MaxOutputTokens { get; init; } = 2048;

        public GenerationConfig ToGenerationConfig() => new()
        {
            Temperature = Temperature,
            TopK = TopK,
            TopP = TopP,
            MaxOutputTokens = MaxOutputTokens
        };
    }
}
