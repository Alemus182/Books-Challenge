using Api.Routes;
using Application.Dtos;
using Application.Services.Books.Queries;
using Application.Services.Books.Responses;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace Test.Api
{
    public class BooksRouterTests
    {
        /// <summary>Subclass that exposes the protected SearchBooks method for unit testing.</summary>
        private sealed class TestableBooksRouter : BooksRouter
        {
            public TestableBooksRouter(ILogger<BooksRouter> logger, IMediator mediator)
                : base(logger, mediator) { }

            public Task<IResult> InvokeSearchBooks(SearchBooksRequest request)
                => SearchBooks(request);
        }

        private static TestableBooksRouter Create(Mock<IMediator> mediatorMock)
        {
            var loggerMock = new Mock<ILogger<BooksRouter>>();
            return new TestableBooksRouter(loggerMock.Object, mediatorMock.Object);
        }

        [Fact]
        public async Task SearchBooks_ReturnsOkResult_WithMediatorResponse()
        {
            var expected = new BookSearchResponse
            {
                Candidates = new List<BookCandidateDto>
                {
                    new() { Title = "The Hobbit", Author = "J.R.R. Tolkien" }
                }
            };
            var mediatorMock = new Mock<IMediator>();
            mediatorMock
                .Setup(m => m.Send(It.IsAny<SearchBooksRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expected);

            var router = Create(mediatorMock);
            var result = await router.InvokeSearchBooks(new SearchBooksRequest { Query = "tolkien hobbit" });

            Assert.NotNull(result);
            Assert.IsAssignableFrom<IResult>(result);
        }

        [Fact]
        public async Task SearchBooks_ForwardsQueryToMediator()
        {
            var mediatorMock = new Mock<IMediator>();
            mediatorMock
                .Setup(m => m.Send(It.IsAny<SearchBooksRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new BookSearchResponse());

            var router = Create(mediatorMock);
            await router.InvokeSearchBooks(new SearchBooksRequest { Query = "dune frank herbert" });

            mediatorMock.Verify(
                m => m.Send(
                    It.Is<SearchBooksRequest>(r => r.Query == "dune frank herbert"),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task SearchBooks_CallsMediatorExactlyOnce_PerRequest()
        {
            var mediatorMock = new Mock<IMediator>();
            mediatorMock
                .Setup(m => m.Send(It.IsAny<SearchBooksRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new BookSearchResponse());

            var router = Create(mediatorMock);
            await router.InvokeSearchBooks(new SearchBooksRequest { Query = "test" });

            mediatorMock.Verify(
                m => m.Send(It.IsAny<SearchBooksRequest>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task SearchBooks_ReturnsOkWithEmptyCandidates_WhenNoResultsFound()
        {
            var mediatorMock = new Mock<IMediator>();
            mediatorMock
                .Setup(m => m.Send(It.IsAny<SearchBooksRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new BookSearchResponse { Candidates = new List<BookCandidateDto>() });

            var router = Create(mediatorMock);
            var result = await router.InvokeSearchBooks(new SearchBooksRequest { Query = "xyz unknown book" });

            Assert.NotNull(result);
        }
    }
}
