using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nameless.Microservice.Infrastructure;
using Nameless.Microservice.Middlewares;

namespace Nameless.Microservice.Extensions {
    public static class ApplicationBuilderExtension {
        #region Public Static Methods

        public static IApplicationBuilder ApplyAuth(this IApplicationBuilder app)
            => app.UseAuthentication()
                .UseAuthorization()
                .UseJwtAuthorization();

        public static IApplicationBuilder ApplyCors(this IApplicationBuilder app)
            => app.UseCors(configure
                => configure
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());

        public static IApplicationBuilder ApplyEndpoints(this IApplicationBuilder app)
            => app.UseEndpoints(configure => {
                var endpoints = app.ApplicationServices.GetServices<IEndpoint>();
                foreach (var endpoint in endpoints) {
                    endpoint.Map(configure);
                }
            });

        public static IApplicationBuilder ApplyHealthCheck(this IApplicationBuilder app)
            => app.UseHealthChecks("/healthz");

        public static IApplicationBuilder ApplyHttpSecurity(this IApplicationBuilder app, IHostEnvironment env) {
            if (!env.IsDevelopment()) {
                // The default HSTS value is 30 days. You may want to change
                // this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            return app;
        }

        public static IApplicationBuilder ApplyRouting(this IApplicationBuilder app)
            => app.UseRouting();

        public static IApplicationBuilder ApplySwagger(this IApplicationBuilder app, IHostEnvironment env, IApiVersionDescriptionProvider versioning) {
            if (env.IsDevelopment()) {
                app.UseSwagger();
                app.UseSwaggerUI(opts => {
                    foreach (var description in versioning.ApiVersionDescriptions) {
                        opts.SwaggerEndpoint(
                            url: $"/swagger/{description.GroupName}/swagger.json",
                            name: description.GroupName.ToUpperInvariant()
                        );
                    }
                });
            }

            return app;
        }

        public static IApplicationBuilder ApplyErrorHandling(this IApplicationBuilder app, IHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseExceptionHandler("/error");
            }

            return app;
        }

        #endregion
    }
}
