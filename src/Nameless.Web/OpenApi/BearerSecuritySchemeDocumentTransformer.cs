using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Nameless.Web.OpenApi;

/// <summary>
///     An OpenAPI document transformer that adds a Bearer token security
///     scheme to the OpenAPI document based on the application's
///     authentication configuration.
///     This enables API consumers to understand and use Bearer
///     authentication when interacting with the API endpoints.
/// </summary>
public class BearerSecuritySchemeDocumentTransformer : IOpenApiDocumentTransformer {
    private readonly IAuthenticationSchemeProvider _authenticationSchemeProvider;

    /// <summary>
    ///     Initializes a new instance of <see cref="BearerSecuritySchemeDocumentTransformer"/>.
    /// </summary>
    /// <param name="authenticationSchemeProvider">
    ///     The authentication scheme provider.
    /// </param>
    public BearerSecuritySchemeDocumentTransformer(IAuthenticationSchemeProvider authenticationSchemeProvider) {
        _authenticationSchemeProvider = authenticationSchemeProvider;
    }

    /// <inheritdoc />
    public async Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken) {
        var authenticationSchemes = await _authenticationSchemeProvider.GetAllSchemesAsync();
        if (authenticationSchemes.All(authScheme => authScheme.Name != JwtBearerDefaults.AuthenticationScheme)) {
            return;
        }

        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();

        document.Components.SecuritySchemes.Add(JwtBearerDefaults.AuthenticationScheme,
            new OpenApiSecurityScheme {
                Type = SecuritySchemeType.Http,
                Scheme = JwtBearerDefaults.AuthenticationScheme.ToLowerInvariant(),
                In = ParameterLocation.Header,
                BearerFormat = "JSON Web Token"
            });

        document.Security ??= [];
        document.Security.Add(new OpenApiSecurityRequirement {
            [new OpenApiSecuritySchemeReference(
                JwtBearerDefaults.AuthenticationScheme
            )] = []
        });
    }
}