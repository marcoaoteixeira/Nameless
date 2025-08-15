using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace Nameless.Web.OpenApi;

/// <summary>
///     An OpenAPI document transformer that adds the "Bearer" token option.
/// </summary>
public sealed class BearerSecuritySchemeDocumentTransformer : IOpenApiDocumentTransformer {
    private readonly IAuthenticationSchemeProvider _authenticationSchemeProvider;

    /// <summary>
    /// Initializes a new instance of <see cref="BearerSecuritySchemeDocumentTransformer"/>.
    /// </summary>
    /// <param name="authenticationSchemeProvider">The authentication scheme provider.</param>
    public BearerSecuritySchemeDocumentTransformer(IAuthenticationSchemeProvider authenticationSchemeProvider) {
        _authenticationSchemeProvider = Guard.Against.Null(authenticationSchemeProvider);
    }

    /// <inheritdoc />
    /// <remarks>
    /// This will add the option to pass a JSON Web Token to all available operations.
    /// </remarks>
    public async Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken) {
        Guard.Against.Null(document);

        var authenticationSchemes = await _authenticationSchemeProvider.GetAllSchemesAsync();
        if (authenticationSchemes.All(authScheme => authScheme.Name != JwtBearerDefaults.AuthenticationScheme)) {
            return;
        }

        document.Components ??= new OpenApiComponents();

        document.Components.SecuritySchemes.Add(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme {
            Type = SecuritySchemeType.Http,
            Scheme = JwtBearerDefaults.AuthenticationScheme.ToLowerInvariant(),
            In = ParameterLocation.Header,
            BearerFormat = "JSON Web Token"
        });

        document.SecurityRequirements.Add(new OpenApiSecurityRequirement {
            [new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Id = JwtBearerDefaults.AuthenticationScheme,
                    Type = ReferenceType.SecurityScheme
                }
            }] = []
        });
    }
}
