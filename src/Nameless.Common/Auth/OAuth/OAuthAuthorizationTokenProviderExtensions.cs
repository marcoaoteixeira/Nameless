using Nameless.ObjectModel;

namespace Nameless.Auth.OAuth;

/// <summary>
///     <see cref="IOAuthAuthorizationTokenProvider"/> extension methods
/// </summary>
public static class OAuthAuthorizationTokenProviderExtensions {
    /// <param name="self">
    ///     The current <see cref="IOAuthAuthorizationTokenProvider"/> instance.
    /// </param>
    extension(IOAuthAuthorizationTokenProvider self) {
        /// <summary>
        ///     Synchronously retrieves the OAuth authentication token.
        /// </summary>
        /// <param name="request">
        ///     The request.
        /// </param>
        /// <param name="timeout">
        ///     The timeout. If not provided, it will wait until completion.
        /// </param>
        /// <returns>
        ///     An <see cref="OAuthAuthorizationTokenResponse"/> instance.
        /// </returns>
        public OAuthAuthorizationTokenResponse GetToken(OAuthAuthorizationTokenRequest request, int timeout = -1) {
            using var cts = new CancellationTokenSource(millisecondsDelay: timeout);

            try { return self.GetTokenAsync(request, cts.Token).GetAwaiter().GetResult(); }
            catch (OperationCanceledException ex) { return Error.Conflict(ex.Message); }
        }
    }
}