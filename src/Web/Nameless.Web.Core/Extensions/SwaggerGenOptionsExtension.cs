using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Nameless.Web.Infrastructure;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Nameless.Web;

public static class SwaggerGenOptionsExtension {
    public static void UseJwtAuthentication(this SwaggerGenOptions self) {
        var openApiSecurityScheme = new OpenApiSecurityScheme {
            In = ParameterLocation.Header,
            Description = "Enter JSON Web Token",
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            BearerFormat = "JWT",
            Scheme = JwtBearerDefaults.AuthenticationScheme.ToLower()
        };
        self.AddSecurityDefinition(name: JwtBearerDefaults.AuthenticationScheme,
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
        self.AddSecurityRequirement(openApiSecurityRequirement);
    }

    public static void UseDefaultValuesOperationFilter(this SwaggerGenOptions self)
        => self.OperationFilter<SwaggerDefaultValuesOperationFilter>();
}
