using Api.Routes;
using Application.Dtos.Auth;
using Application.Services.Auth.Commands;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace Test.Api
{
    public class AuthRouterTests
    {
        /// <summary>Subclass that exposes the protected Post method for unit testing.</summary>
        private sealed class TestableAuthRouter : AuthRouter
        {
            public TestableAuthRouter(ILogger<AuthRouter> logger, IMediator mediator)
                : base(logger, mediator) { }

            public Task<IResult> InvokePost(SingInRequest request)
                => Post(request);
        }

        private static TestableAuthRouter Create(Mock<IMediator> mediatorMock)
        {
            var loggerMock = new Mock<ILogger<AuthRouter>>();
            return new TestableAuthRouter(loggerMock.Object, mediatorMock.Object);
        }

        [Fact]
        public async Task Post_ReturnsOkResult_WithMediatorResponse()
        {
            var expected = new SingInResponse
            {
                valid = true,
                id = "user@test.com",
                token = "jwt-token-sample",
                refreshToken = "refresh-token-sample"
            };
            var mediatorMock = new Mock<IMediator>();
            mediatorMock
                .Setup(m => m.Send(It.IsAny<SingInRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expected);

            var router = Create(mediatorMock);
            var result = await router.InvokePost(new SingInRequest { userName = "user@test.com", password = "pass" });

            Assert.NotNull(result);
            Assert.IsAssignableFrom<IResult>(result);
        }

        [Fact]
        public async Task Post_ForwardsCredentialsToMediator()
        {
            var mediatorMock = new Mock<IMediator>();
            mediatorMock
                .Setup(m => m.Send(It.IsAny<SingInRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new SingInResponse());

            var router = Create(mediatorMock);
            await router.InvokePost(new SingInRequest { userName = "admin@test.com", password = "secret" });

            mediatorMock.Verify(
                m => m.Send(
                    It.Is<SingInRequest>(r => r.userName == "admin@test.com" && r.password == "secret"),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Post_CallsMediatorExactlyOnce_PerRequest()
        {
            var mediatorMock = new Mock<IMediator>();
            mediatorMock
                .Setup(m => m.Send(It.IsAny<SingInRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new SingInResponse());

            var router = Create(mediatorMock);
            await router.InvokePost(new SingInRequest { userName = "user", password = "pass" });

            mediatorMock.Verify(
                m => m.Send(It.IsAny<SingInRequest>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
