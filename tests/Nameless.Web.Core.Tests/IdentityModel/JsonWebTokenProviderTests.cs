using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Nameless.Testing.Tools.Attributes;
using Nameless.Web.IdentityModel.Jwt;

namespace Nameless.Web.IdentityModel;

[UnitTest]
public class JsonWebTokenProviderTests {
    [Fact]
    public void WhenCreating_WithValidClaims_ThenReturnsToken() {
        // arrange
        var options = Options.Create(new JsonWebTokenOptions {
            Secret = "36d9bbef-9c71-4065-a2f5-b40fabd642e0"
        });
        var sut = new JsonWebTokenProvider(TimeProvider.System, options, NullLogger<JsonWebTokenProvider>.Instance);

        // act
        var actual = sut.Create(new JsonWebTokenRequest {
            Claims = [new Claim("sub", "cbf4d0e9-a1a3-4742-b4b6-a9c459e7b288")]
        });

        // assert
        Assert.Multiple(() => {
            Assert.NotNull(actual);

            if (!actual.Succeeded) {
                Assert.Fail(actual.Error);
            }

            Assert.True(actual.Succeeded);
            Assert.NotNull(actual.Token);
        });
    }

    [Fact]
    public void ValidateToken() {
        // arrange
        var options = Options.Create(new JsonWebTokenOptions {
            Secret = "36d9bbef-9c71-4065-a2f5-b40fabd642e0"
        });
        var sut = new JsonWebTokenProvider(TimeProvider.System, options, NullLogger<JsonWebTokenProvider>.Instance);

        // act
        var actual = sut.Create(new JsonWebTokenRequest {
            Claims = [new Claim("sub", "cbf4d0e9-a1a3-4742-b4b6-a9c459e7b288")]
        });

        if (!actual.Succeeded) {
            Assert.Fail(actual.Error);
        }

        var parts = actual.Token.Split(".");

        var raw = string.Join(".", parts[0], parts[1]).GetBytes();
        var hashAlg = new HMACSHA256(options.Value.Secret.GetBytes());
        var hash = hashAlg.ComputeHash(raw);
        var signature = WebEncoders.Base64UrlEncode(hash);

        if (parts[2] != signature) {
            Assert.Fail("Invalid signature");
        }

        // assert
        Assert.Multiple(() => {
            Assert.NotNull(actual);

            if (!actual.Succeeded) {
                Assert.Fail(actual.Error);
            }

            Assert.True(actual.Succeeded);
            Assert.NotNull(actual.Token);
        });
    }
}
