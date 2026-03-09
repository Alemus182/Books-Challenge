using Api.Components;
using Application.Services.Auth.Commands;
using MediatR;

namespace Api.Routes
{
    public class AuthRouter : RouterBase
    {
        public AuthRouter(ILogger<AuthRouter> logger, IMediator mediator)
        {
            Logger = logger;
            Mediator = mediator;
        }

        public override void AddRoutes(WebApplication app)
        {
            var group = app.MapGroup(ApiRoutes.Auth.Login).WithOpenApi();
            group.MapPost($"/", (SingInRequest SingIn) => Post(SingIn));
        }

        protected virtual async Task<IResult> Post(SingInRequest SingIn)
        {
            var result = await Mediator?.Send(SingIn);
            return Results.Ok(result);
        }
    }
}
