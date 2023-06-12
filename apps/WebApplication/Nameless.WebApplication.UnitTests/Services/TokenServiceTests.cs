using System.Net;
using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Nameless.WebApplication.Entities;
using Nameless.WebApplication.Options;
using Nameless.WebApplication.Services.Impl;
using NSubstitute;
using MS_Options = Microsoft.Extensions.Options.Options;

namespace Nameless.WebApplication.UnitTests.Services {

    public class TokenServiceTests {

        private Guid _userId;
        private ApplicationDbContext _dbContext;
        private IHttpContextAccessor _httpContextAccessor;
        private string _secret = "4b1e7902-f6a1-4693-86be-b39e319a2a6e";

        [OneTimeSetUp]
        public void OneTimeSetUp() {
            _userId = Guid.Parse("77425b10-176d-427a-9fb3-465dfa4db073");
            _dbContext = DbContextFactory.CreateInMemory();
            _httpContextAccessor = Substitute.For<IHttpContextAccessor>();

            // DbContext setup
            _dbContext.Users.Add(new User {
                Id = _userId,
                UserName = "Test",
                Email = "test@test.com",
            });
            _dbContext.SaveChanges();


            // HttpContextAccessor setup
            var httpContext = Substitute.For<HttpContext>();
            httpContext.Request.Returns(Substitute.For<HttpRequest>());
            httpContext.Request.Headers.Returns(new HeaderDictionary());
            httpContext.Connection.Returns(Substitute.For<ConnectionInfo>());
            httpContext.Connection.RemoteIpAddress.Returns(IPAddress.Loopback);
            _httpContextAccessor.HttpContext = Substitute.For<HttpContext>();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown() {
            _dbContext.Dispose();
        }

        [Test]
        public async Task GenerateAsync_Generates_A_New_Token() {
            // arrange
            var options = new JsonWebTokenOptions { Secret = _secret };
            var tokenService = new AccessTokenService(MS_Options.Create(options));

            // act
            var token = await tokenService.GenerateAsync(_userId, "Test", "test@test.com", CancellationToken.None);

            // assert
            token.Should().NotBeNull();
        }

        [Test]
        public async Task ExtractAsync_Retrieves_ClaimsPrincipal_From_Token() {
            // arrange
            var options = new JsonWebTokenOptions { Secret = _secret };
            var tokenService = new AccessTokenService(MS_Options.Create(options));
            var token = await tokenService.GenerateAsync(_userId, "Test", "test@test.com", CancellationToken.None);

            // act
            var principal = await tokenService.ExtractAsync(token, CancellationToken.None);

            // assert
            principal.Should().NotBeNull();
            principal.Claims.Should().Contain(_ => _.Type == ClaimTypes.NameIdentifier);
            principal.Claims.Single(_ => _.Type == ClaimTypes.NameIdentifier).Value.Should().Be(_userId.ToString());
        }

        [Test]
        public async Task ExtractAsync_Outdated_Token_Fails() {
            // arrange
            var options = new JsonWebTokenOptions { Secret = _secret, ValidateLifetime = true, AccessTokenTtl = 1 /* seconds */ };
            var tokenService = new AccessTokenService(MS_Options.Create(options));

            var token = await tokenService.GenerateAsync(_userId, "Test", "test@test.com", CancellationToken.None);
            await Task.Delay(1500); /* 1.5 seconds */

            // act && assert
            Assert.That(async () => await tokenService.ExtractAsync(token, CancellationToken.None), Throws.InstanceOf<SecurityTokenExpiredException>());
        }
    }
}
