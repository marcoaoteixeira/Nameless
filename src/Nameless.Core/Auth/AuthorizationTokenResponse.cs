using Nameless.ObjectModel;
using Nameless.Results;

namespace Nameless.Auth;

public sealed class AuthorizationTokenResponse<TToken> : Result<TToken>
    where TToken : notnull {
    private AuthorizationTokenResponse(TToken? value, Error[] errors)
        : base(value, errors) {
    }

    public static implicit operator AuthorizationTokenResponse<TToken>(TToken value) {
        return new AuthorizationTokenResponse<TToken>(value, errors: []);
    }

    public static implicit operator AuthorizationTokenResponse<TToken>(Error error) {
        return new AuthorizationTokenResponse<TToken>(value: default, errors: [error]);
    }
}