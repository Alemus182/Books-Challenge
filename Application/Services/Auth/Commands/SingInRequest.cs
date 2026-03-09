#nullable disable

using MediatR;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using Application.Dtos.Auth;
using Application.Interfaces.Services;

namespace Application.Services.Auth.Commands
{
    public class SingInRequest : IRequest<SingInResponse>
    {
        public string userName { get; set; }
        public string password { get; set; }

        public class SignInHandler : IRequestHandler<SingInRequest, SingInResponse>
        {
            private readonly ITokenService _tokenService;

            public SignInHandler(ITokenService tokenService, IConfiguration configuration)
            {
                _tokenService = tokenService;
            }

            public async Task<SingInResponse> Handle(SingInRequest request, CancellationToken cancellationToken)
            {
                var response = new SingInResponse();
                response.id = request.userName;
                response = GenerateAuthenticationResultForUserAsync(response);                
                return response;
            }

            private SingInResponse GenerateAuthenticationResultForUserAsync(SingInResponse response)
            {
                var usersClaims = new [] 
                {
                        new Claim(JwtRegisteredClaimNames.Sub, response.id),
                        new Claim(JwtRegisteredClaimNames.NameId,response.id),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Email, response.id),
                 };

                var token = _tokenService.GenerateAccessToken(usersClaims);
                var refreshToken = _tokenService.GenerateRefreshToken();

                response.valid = true;
                response.token = token;
                response.refreshToken = refreshToken;
                return response;
            }
        }
    }
}