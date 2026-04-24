using Nameless.ObjectModel;

namespace Nameless.Auth;

/// <summary>
///     <see cref="IAuthorizationTokenProvider{TRequest,TToken}"/> extension methods
/// </summary>
public static class AuthorizationTokenProviderExtensions {
    /// <typeparam name="TRequest">
    ///     Type of the request.
    /// </typeparam>
    /// <typeparam name="TToken">
    ///     Type of the response.
    /// </typeparam>
    /// <param name="self">
    ///     The current <see cref="IAuthorizationTokenProvider{TRequest,TToken}"/>
    ///     instance.
    /// </param>
    extension<TRequest, TToken>(IAuthorizationTokenProvider<TRequest, TToken> self)
        where TRequest : AuthorizationTokenRequest
        where TToken : notnull {
        /// <summary>
        ///     Retrieves a token from the provider.
        /// </summary>
        /// <param name="request">
        ///     The request.
        /// </param>
        /// <param name="timeout">
        ///     The timeout. Value <![CDATA[-1]]> means wait undefinedly.
        /// </param>
        /// <returns>
        ///     A <see cref="AuthorizationTokenResponse{TToken}"/> representing
        ///     the authorization token response.
        /// </returns>
        AuthorizationTokenResponse<TToken> GetToken(TRequest request, int timeout = -1) {
            using var cts = new CancellationTokenSource(timeout);

            try { return self.GetTokenAsync(request, cts.Token).GetAwaiter().GetResult(); }
            catch (Exception ex) { return Error.Failure(ex.Message); }
        }
    }
}