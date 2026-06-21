namespace Nameless.Auth;

/// <summary>
///     Provides means to retrieve an authorization token.
/// </summary>
/// <typeparam name="TRequest">
///     Type of the request.
/// </typeparam>
/// <typeparam name="TResponse">
///     Type of the response.
/// </typeparam>
public interface IAuthorizationTokenProvider<in TRequest, TResponse>
    where TRequest : IAuthorizationTokenRequest<TResponse>
    where TResponse : notnull {
    /// <summary>
    ///     Retrieves a token from the provider.
    /// </summary>
    /// <param name="request">
    ///     The request.
    /// </param>
    /// <param name="cancellationToken">
    ///     The cancellation token.
    /// </param>
    /// <returns>
    ///     A <see cref="Task{TResult}"/> representing the asynchronous
    ///     method execution, where the result is an authorization token
    ///     response.
    /// </returns>
    Task<TResponse> GetTokenAsync(TRequest request, CancellationToken cancellationToken);
}