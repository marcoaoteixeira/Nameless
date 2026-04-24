using Nameless.ObjectModel;

namespace Nameless.Auth;

public class AuthorizationTokenTests {
    // --- AuthorizationTokenRequest ---

    [Fact]
    public void AuthorizationTokenRequest_DefaultScheme_IsNull() {
        // act
        var request = new AuthorizationTokenRequest();

        // assert
        Assert.Null(request.Scheme);
    }

    [Fact]
    public void AuthorizationTokenRequest_WithScheme_SetsScheme() {
        // act
        var request = new AuthorizationTokenRequest { Scheme = "Bearer" };

        // assert
        Assert.Equal("Bearer", request.Scheme);
    }

    // --- AuthorizationTokenResponse<T> ---

    [Fact]
    public void AuthorizationTokenResponse_FromValue_IsSuccess() {
        // act
        AuthorizationTokenResponse<string> response = "token-abc";

        // assert
        Assert.Multiple(() => {
            Assert.True(response.Success);
            Assert.Equal("token-abc", response.Value);
        });
    }

    [Fact]
    public void AuthorizationTokenResponse_FromError_IsFailure() {
        // act
        AuthorizationTokenResponse<string> response = Error.Failure("invalid");

        // assert
        Assert.Multiple(() => {
            Assert.False(response.Success);
            Assert.Single(response.Errors);
        });
    }
}
