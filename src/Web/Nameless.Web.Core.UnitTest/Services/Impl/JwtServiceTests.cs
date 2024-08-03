using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.IdentityModel.JsonWebTokens;
using Nameless.Services.Impl;
using Nameless.Web.Options;

namespace Nameless.Web.Services.Impl {
    public class JwtServiceTests {
        [Test]
        public void Generate_Should_Return_Token() {
            // arrange
            var jwtClaims = new JwtClaims {
                Sub = "123456",
                Name = "Test User",
                Email = "test_user@test.com"
            };
            var sut = new JwtService(options: new JwtOptions(),
                                     systemClock: SystemClock.Instance,
                                     logger: NullLogger<JwtService>.Instance);

            // act
            var actual = sut.Generate(jwtClaims);

            // assert
            Assert.That(actual, Is.Not.Null);
        }

        [Test]
        public void Validate_Should_Return_True_And_ClaimsPrincipal_When_Valid_Token() {
            // arrange
            var jwtClaims = new JwtClaims {
                Sub = "123456",
                Name = "Test User",
                Email = "test_user@test.com"
            };
            var sut = new JwtService(options: new JwtOptions(),
                                     systemClock: SystemClock.Instance,
                                     logger: NullLogger<JwtService>.Instance);

            // act
            var token = sut.Generate(jwtClaims);

            var valid = sut.TryValidate(token, out var principal);

            // assert
            Assert.Multiple(() => {
                Assert.That(token, Is.Not.Null);
                Assert.That(valid, Is.True);
                Assert.That(principal, Is.Not.Null);

                var sub = principal.Claims.SingleOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Sub);
                Assert.That(sub, Is.Not.Null);
                Assert.That(sub.Value, Is.EqualTo(jwtClaims.Sub));

                var name = principal.Claims.SingleOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Name);
                Assert.That(name, Is.Not.Null);
                Assert.That(name.Value, Is.EqualTo(jwtClaims.Name));

                var email = principal.Claims.SingleOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Email);
                Assert.That(email, Is.Not.Null);
                Assert.That(email.Value, Is.EqualTo(jwtClaims.Email));
            });
        }
    }
}
