using Application.Interfaces.Infraestructure.Services;
using Infraestructure.Services;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Security.Claims;

namespace Test.Infrastructure
{
    public class CurrentUserServiceTests
    {
        private static ICurrentUserService CreateService(string? userId = null, string? email = null)
        {
            var claims = new List<Claim>();
            if (userId != null) claims.Add(new Claim(ClaimTypes.NameIdentifier, userId));
            if (email != null) claims.Add(new Claim(ClaimTypes.Email, email));

            var identity = new ClaimsIdentity(claims, "Test");
            var principal = new ClaimsPrincipal(identity);

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(h => h.User).Returns(principal);

            var accessor = new Mock<IHttpContextAccessor>();
            accessor.Setup(a => a.HttpContext).Returns(httpContext.Object);

            return new CurrentUserService(accessor.Object);
        }

        [Fact]
        public void UserId_IsSet_WhenNameIdentifierClaimExists()
        {
            var service = CreateService(userId: "abc-123");
            Assert.Equal("abc-123", service.UserId);
        }

        [Fact]
        public void Username_IsSet_WhenEmailClaimExists()
        {
            var service = CreateService(email: "user@test.com");
            Assert.Equal("user@test.com", service.Username);
        }

        [Fact]
        public void IsAuthenticated_IsTrue_WhenUserIdIsPresent()
        {
            var service = CreateService(userId: "abc-123");
            Assert.True(service.IsAuthenticated);
        }

        [Fact]
        public void IsAuthenticated_IsFalse_WhenNoUserIdClaim()
        {
            var service = CreateService();
            Assert.False(service.IsAuthenticated);
        }

        [Fact]
        public void AllProperties_AreNull_WhenHttpContextIsNull()
        {
            var accessor = new Mock<IHttpContextAccessor>();
            accessor.Setup(a => a.HttpContext).Returns((HttpContext?)null);

            var service = new CurrentUserService(accessor.Object);

            Assert.Null(service.UserId);
            Assert.Null(service.Username);
            Assert.False(service.IsAuthenticated);
        }

        [Fact]
        public void Username_IsNull_WhenOnlyUserIdClaimExists()
        {
            var service = CreateService(userId: "user-99");
            Assert.Null(service.Username);
        }

        [Fact]
        public void UserId_IsNull_WhenOnlyEmailClaimExists()
        {
            var service = CreateService(email: "only@email.com");
            Assert.Null(service.UserId);
        }
    }
}
