using Microsoft.Extensions.DependencyInjection;
using Nameless.Auth;
using Nameless.Contracts;

namespace Nameless.Hosting.Features.Auth;

/// <summary>
///     Access Token Provider Feature Options
/// </summary>
/// <typeparam name="TRequest">
///     Type of the request.
/// </typeparam>
public class AccessTokenProviderFeatureOptions<TRequest> where TRequest : notnull {
    /// <summary>
    ///     Whether it should use caching to store for a while the token
    ///     before require it again. Default is <c>true</c>.
    /// </summary>
    public bool UseCache { get; private set; } = true;

    /// <summary>
    ///     Gets the request comparer.
    /// </summary>
    public IEqualityComparer<TRequest>? RequestComparer { get; private set; }

    /// <summary>
    ///     Gets the delegate that configures the HTTP client for the
    ///     <see cref="IAccessTokenProvider{TRequest}"/>.
    /// </summary>
    public Action<IHttpClientBuilder>? ConfigureHttpClient { get; private set; }

    /// <summary>
    ///     Sets whether it should use caching to store for a while the token
    ///     before require it again.
    /// </summary>
    /// <param name="value">
    ///     The value.
    /// </param>
    /// <returns>
    ///     The current instance of <see cref="AccessTokenProviderFeatureOptions{TRequest}"/>
    ///     so other actions can be chained.
    /// </returns>
    public AccessTokenProviderFeatureOptions<TRequest> WithUseCase(bool value) {
        UseCache = value;

        return this;
    }

    /// <summary>
    ///     Sets the request object equality comparer.
    /// </summary>
    /// <param name="value">
    ///     The value.
    /// </param>
    /// <returns>
    ///     The current instance of <see cref="AccessTokenProviderFeatureOptions{TRequest}"/>
    ///     so other actions can be chained.
    /// </returns>
    public AccessTokenProviderFeatureOptions<TRequest> WithRequestComparer(IEqualityComparer<TRequest> value) {
        RequestComparer = Throws.When.Null(value);

        return this;
    }

    /// <summary>
    ///     Sets the delegate that configures the HTTP client for the
    ///     <see cref="IAccessTokenProvider{TRequest}"/>.
    /// </summary>
    /// <param name="value">
    ///     The value.
    /// </param>
    /// <returns>
    ///     The current instance of <see cref="AccessTokenProviderFeatureOptions{TRequest}"/>
    ///     so other actions can be chained.
    /// </returns>
    public AccessTokenProviderFeatureOptions<TRequest> WithHttpClientConfiguration(Action<IHttpClientBuilder> value) {
        ConfigureHttpClient += Throws.When.Null(value);

        return this;
    }
}