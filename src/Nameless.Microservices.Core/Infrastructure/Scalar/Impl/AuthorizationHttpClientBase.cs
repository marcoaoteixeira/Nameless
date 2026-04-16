using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;

namespace Nameless.Microservices.Infrastructure.Scalar.Impl;

public abstract class AuthorizationHttpClientBase<TOptions> : IAuthorizationHttpClient
    where TOptions : AuthorizationSchemeOptions {
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;

    protected AuthorizationHttpClientBase(IConfiguration configuration, HttpClient httpClient) {
        _configuration = configuration;
        _httpClient = httpClient;
    }

    public async Task<TToken> GetTokenAsync<TToken>(string authorizationScheme, CancellationToken cancellationToken) {
        // we'll look into a section named AuthorizationSchemes to find
        // the specified scheme
        var section = _configuration.GetSection($"AuthorizationSchemes:{authorizationScheme}");
        var options = section.Get<TOptions>();

        if (options is null) {
            throw new InvalidOperationException(
                $"Authorization scheme options for '{authorizationScheme}' not found in section 'AuthorizationSchemes'."
            );
        }

        var content = CreateHttpContent(options);
        var response = await _httpClient.PostAsync(
            options.Issuer,
            content,
            cancellationToken
        ).SkipContextSync();

        response.EnsureSuccessStatusCode();

        var token = await response.Content
                                  .ReadFromJsonAsync<TToken>(cancellationToken)
                                  .SkipContextSync();

        return token ?? throw new InvalidOperationException(
            $"Couldn't convert response JSON to token type '{typeof(TToken).Name}'."
        );
    }

    protected abstract HttpContent CreateHttpContent(TOptions options);
}