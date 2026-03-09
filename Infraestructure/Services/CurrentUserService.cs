using System.Security.Claims;
using Application.Interfaces.Infraestructure.Services;
using Microsoft.AspNetCore.Http;

namespace Infraestructure.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        public CurrentUserService
            (
            IHttpContextAccessor httpContextAccessor
            )
        {
            UserId = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            Username = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value;
            IsAuthenticated = UserId != null;
        }

        public string UserId { get; }
        public string Username { get; }
        public bool IsAuthenticated { get; }
    }
}
