using Application.Interfaces.Infraestructure.Services;
using Application.Models;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Infraestructure.Services
{
    public class PromptProvider : IPromptProvider
    {
        private readonly ILogger<PromptProvider> _logger;
        private readonly PromptConfigurationFile _prompts;
        private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

        public PromptProvider(string promptFilePath, ILogger<PromptProvider> logger)
        {
            _logger = logger;
            
            if (!File.Exists(promptFilePath))
            {
                _logger.LogWarning("Prompt file not found at {Path}, using default prompts", promptFilePath);
                _prompts = GetDefaultPrompts();
                return;
            }

            try
            {
                var json = File.ReadAllText(promptFilePath);
                _prompts = JsonSerializer.Deserialize<PromptConfigurationFile>(json, JsonOptions)
                          ?? GetDefaultPrompts();
                _logger.LogInformation("Loaded prompts from {Path}", promptFilePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load prompts from {Path}, using defaults", promptFilePath);
                _prompts = GetDefaultPrompts();
            }
        }

        public PromptTemplate GetExtractFieldsPrompt()
        {
            var def = _prompts.ExtractFields ?? throw new InvalidOperationException("ExtractFields prompt not configured");
            return new PromptTemplate
            {
                Version = def.Version,
                Template = def.Template,
                Config = def.Config?.ToGenerationConfig() ?? new GenerationConfig()
            };
        }

        public PromptTemplate GetRerankCandidatesPrompt()
        {
            var def = _prompts.RerankCandidates ?? throw new InvalidOperationException("RerankCandidates prompt not configured");
            return new PromptTemplate
            {
                Version = def.Version,
                Template = def.Template,
                Config = def.Config?.ToGenerationConfig() ?? new GenerationConfig()
            };
        }

        public Dictionary<string, string> GetVersionHistory()
        {
            return _prompts.VersionHistory ?? new Dictionary<string, string>();
        }

        private static PromptConfigurationFile GetDefaultPrompts()
        {
            return new PromptConfigurationFile
            {
                ExtractFields = new PromptDefinition
                {
                    Version = "1.0",
                    Template = "Extract structured fields from this book search query.\nReturn ONLY valid JSON: {\"title\": \"...\", \"author\": \"...\", \"keywords\": [\"...\"]}\n\nQuery: {0}",
                    Config = new GenerationConfigJson { Temperature = 0.2, TopK = 40, TopP = 0.95, MaxOutputTokens = 512 }
                },
                RerankCandidates = new PromptDefinition
                {
                    Version = "1.0",
                    Template = "Rerank these books by relevance.\nReturn JSON: [{\"openLibraryId\": \"...\", \"explanation\": \"...\"}]\n\nQuery: {0}\nCandidates: {1}",
                    Config = new GenerationConfigJson { Temperature = 0.3, TopK = 40, TopP = 0.9, MaxOutputTokens = 1024 }
                },
                VersionHistory = new Dictionary<string, string>
                {
                    ["1.0"] = "Default fallback prompts"
                }
            };
        }
    }
}
