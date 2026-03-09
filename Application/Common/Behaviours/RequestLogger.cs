using Application.Interfaces.Infraestructure.Services;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;

namespace Application.Common.Behaviours
{
    public class RequestLogger<TRequest> : IRequestPreProcessor<TRequest> where TRequest: notnull 
    {
        private readonly ILogger _logger;
        private readonly ICurrentUserService _currentUserService;

        public RequestLogger(ILogger<TRequest> logger, ICurrentUserService currentUserService)
        {
            _logger = logger;
            _currentUserService = currentUserService;
        }

        public Task Process(TRequest request, CancellationToken cancellationToken)
        {
            var name = typeof(TRequest).Name;

            _logger.LogInformation("Request: {Name} / UserId : {@UserId} / Autenticated : {@IsAuthenticated} / Path : {@Request}",
                name, _currentUserService.UserId, _currentUserService.IsAuthenticated, request);

            return Task.CompletedTask;
        }
    }
}
