namespace Nameless.Auth.OAuth;

/// <summary>
///     <see cref="IAuthorizationTokenProvider{TRequest,TResponse}"/> extension methods
/// </summary>
public static class AuthorizationTokenProviderExtensions {
    /// <param name="self">
    ///     The current <see cref="IAuthorizationTokenProvider{TRequest,TResponse}"/> instance.
    /// </param>
    extension<TRequest, TResponse>(IAuthorizationTokenProvider<TRequest, TResponse> self)
        where TRequest : notnull
        where TResponse : notnull {
        /// <summary>
        ///     Synchronously retrieves the authentication token.
        /// </summary>
        /// <param name="request">
        ///     The request.
        /// </param>
        /// <param name="timeout">
        ///     The timeout. If not provided, it will wait until completion.
        /// </param>
        /// <returns>
        ///     An <typeparamref name="TResponse"/> instance.
        /// </returns>
        public TResponse GetToken(TRequest request, int timeout = -1) {
            using var cts = new CancellationTokenSource(millisecondsDelay: timeout);

            return self.GetTokenAsync(request, cts.Token).GetAwaiter().GetResult();
        }
    }
}