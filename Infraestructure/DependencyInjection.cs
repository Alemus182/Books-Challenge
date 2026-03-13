using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Infraestructure.Services;
using Application.Interfaces.Services;
using Application.Interfaces.Infraestructure.Services;

namespace Infraestructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration, string? promptFilePath = null)
        {
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IOpenLibraryService, OpenLibraryService>();
            services.AddScoped<IGeminiService, GeminiService>();

            services.AddSingleton<IPromptProvider>(sp =>
            {
                var logger = sp.GetRequiredService<Microsoft.Extensions.Logging.ILogger<PromptProvider>>();
                var resolvedPath = promptFilePath ?? configuration["PromptSettings:FilePath"] ?? "prompts.json";
                return new PromptProvider(resolvedPath, logger);
            });

            return services;
        }
    }
}
