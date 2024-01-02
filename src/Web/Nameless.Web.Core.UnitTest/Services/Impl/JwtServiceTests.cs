using System.Security.Claims;

namespace Nameless.Web.Services.Impl {
    public class JwtServiceTests {
        [Test]
        public void Generate_Should_Return_Token() {
            // arrange
            var sut = new JwtService();

            // act
            var actual = sut.Generate([
                new(ClaimTypes.NameIdentifier, "123456"),
                new(ClaimTypes.Name, "Test User"),
                new(ClaimTypes.Email, "test@test.com"),
            ]);

            // assert
            Assert.That(actual, Is.Not.Null);
        }

        [Test]
        public void Validate_Should_Return_True_And_ClaimsPrincipal_When_Valid_Token() {
            // arrange
            var claims = new Dictionary<string, string> {
                { ClaimTypes.NameIdentifier, "123456" },
                { ClaimTypes.Name, "Test User" },
                { ClaimTypes.Email, "test@test.com" }
            };
            var sut = new JwtService();

            // act
            var token = sut.Generate([
                new(ClaimTypes.NameIdentifier, claims[ClaimTypes.NameIdentifier]),
                new(ClaimTypes.Name, claims[ClaimTypes.Name]),
                new(ClaimTypes.Email, claims[ClaimTypes.Email]),
            ]);

            var valid = sut.Validate(token, out var principal);

            // assert
            Assert.Multiple(() => {
                Assert.That(token, Is.Not.Null);
                Assert.That(valid, Is.True);
                Assert.That(principal, Is.Not.Null);

                var id = principal.Claims.SingleOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier);
                Assert.That(id, Is.Not.Null);
                Assert.That(id.Value, Is.EqualTo(claims[ClaimTypes.NameIdentifier]));

                var name = principal.Claims.SingleOrDefault(claim => claim.Type == ClaimTypes.Name);
                Assert.That(name, Is.Not.Null);
                Assert.That(name.Value, Is.EqualTo(claims[ClaimTypes.Name]));
                
                var email = principal.Claims.SingleOrDefault(claim => claim.Type == ClaimTypes.Email);
                Assert.That(email, Is.Not.Null);
                Assert.That(email.Value, Is.EqualTo(claims[ClaimTypes.Email]));
            });
        }
    }
}
