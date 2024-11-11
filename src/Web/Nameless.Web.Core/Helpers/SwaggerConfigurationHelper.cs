using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Nameless.Web.Helpers;

public static class SwaggerConfigurationHelper {
    public static void ConfigureJwtAuthentication(SwaggerGenOptions options) {
        var openApiSecurityScheme = new OpenApiSecurityScheme {
            In = ParameterLocation.Header,
            Description = "Enter JSON Web Token",
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            BearerFormat = "JWT",
            Scheme = JwtBearerDefaults.AuthenticationScheme.ToLower()
        };
        options.AddSecurityDefinition(name: JwtBearerDefaults.AuthenticationScheme,
                                      securityScheme: openApiSecurityScheme);

        var openApiSecurityRequirement = new OpenApiSecurityRequirement {
            {
                new OpenApiSecurityScheme {
                    Reference = new OpenApiReference {
                        Type = ReferenceType.SecurityScheme,
                        Id = JwtBearerDefaults.AuthenticationScheme
                    }
                },
                []
            }
        };
        options.AddSecurityRequirement(openApiSecurityRequirement);
    }
}
