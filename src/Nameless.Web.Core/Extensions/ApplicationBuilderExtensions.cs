using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

namespace Nameless.Web;

public static class ApplicationBuilderExtensions {
    public static TApplicationBuilder UseHsts<TApplicationBuilder>(this TApplicationBuilder self, IHostEnvironment environment)
        where TApplicationBuilder : IApplicationBuilder {

        // Enables HSTS (HTTP Strict Transport Security) in non-development environments
        if (!environment.IsDevelopment()) {
            self.UseHsts();
        }

        return self;
    }
}
