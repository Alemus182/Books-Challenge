using Application.Models;

namespace Application.Interfaces.Infraestructure.Services
{
    public interface IPromptProvider
    {
        PromptTemplate GetExtractFieldsPrompt();
        PromptTemplate GetRerankCandidatesPrompt();
        Dictionary<string, string> GetVersionHistory();
    }
}
