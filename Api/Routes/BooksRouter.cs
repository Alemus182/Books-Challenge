using Api.Components;
using Application.Services.Books.Queries;
using Application.Services.Books.Responses;
using MediatR;

namespace Api.Routes
{
    public class BooksRouter : RouterBase
    {
        public BooksRouter(ILogger<BooksRouter> logger, IMediator mediator)
        {
            Logger = logger;
            Mediator = mediator;
        }

        public override void AddRoutes(WebApplication app)
        {
            var group = app.MapGroup(ApiRoutes.BooksRoutes.Group).WithOpenApi();

            group.MapPost(ApiRoutes.BooksRoutes.SearchBooks, (SearchBooksRequest req) => SearchBooks(req))
                .Produces<BookSearchResponse>()
                .Produces(400)
                .Produces(500);
        }

        protected virtual async Task<IResult> SearchBooks(SearchBooksRequest req)
        {
            Logger?.LogInformation("Searching books for query: {Query}", req.Query);
            var result = await Mediator?.Send(req);
            return Results.Ok(result);
        }
    }
}
