using Scalar.AspNetCore;

namespace Nameless.Identity.Web.Configs;

public static class OpenApiConfig {
    public static WebApplication UseOpenApiServices(this WebApplication self) {
        if (!self.Environment.IsDevelopment()) {
            return self;
        }

        self.MapOpenApi()
            .CacheOutput(OutputCachePolicy.OpenApi.PolicyName);

        self.MapScalarApiReference(options => {
            options.Title = "Identity App";
            options.Theme = ScalarTheme.BluePlanet;
            options.DefaultHttpClient = new KeyValuePair<ScalarTarget, ScalarClient>(ScalarTarget.CSharp, ScalarClient.HttpClient);
        });

        return self;
    }
}
