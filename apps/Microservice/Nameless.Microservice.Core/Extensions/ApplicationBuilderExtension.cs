using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nameless.Microservice.Infrastructure;
using Nameless.Microservice.Middlewares;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Nameless.Microservice.Extensions {
    public static class ApplicationBuilderExtension {
        #region Public Static Methods

        public static IApplicationBuilder ResolveAuth(this IApplicationBuilder app)
            => app.UseAuthorization()
                .UseAuthentication()
                .UseJwtAuthorization();

        public static IApplicationBuilder ResolveCors(this IApplicationBuilder app)
            => app.UseCors(configure
                => configure
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());

        public static IApplicationBuilder ResolveEndpoints(this IApplicationBuilder app)
            => ResolveEndpoints(app, setup => {
                var endpoints = app.ApplicationServices.GetServices<IEndpoint>();
                foreach (var endpoint in endpoints) {
                    endpoint.Map(setup);
                }
            });

        public static IApplicationBuilder ResolveEndpoints(this IApplicationBuilder app, Action<IEndpointRouteBuilder> setup)
            => app.UseEndpoints(setup);

        public static IApplicationBuilder ResolveHealthChecks(this IApplicationBuilder app)
            => app.UseHealthChecks("/healthz");

        public static IApplicationBuilder ResolveHealthChecks(this IApplicationBuilder app, PathString path, int port, HealthCheckOptions options)
            => app.UseHealthChecks(path, port, options);

        public static IApplicationBuilder ResolveHttpSecurity(this IApplicationBuilder app, IHostEnvironment env) {
            if (!env.IsDevelopment()) {
                // The default HSTS value is 30 days. You may want to change
                // this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            return app;
        }

        public static IApplicationBuilder ResolveRouting(this IApplicationBuilder app)
            => app.UseRouting();

        public static IApplicationBuilder ResolveSwagger(this IApplicationBuilder app, IHostEnvironment env)
            => ResolveSwagger(app, env, setupSwagger => { }, setupSwaggerUI => { });

        public static IApplicationBuilder ResolveSwagger(this IApplicationBuilder app, IHostEnvironment env, IApiVersionDescriptionProvider versioning)
            => ResolveSwagger(app, env, setupSwagger => { }, setupSwaggerUI => {
                foreach (var description in versioning.ApiVersionDescriptions) {
                    setupSwaggerUI.SwaggerEndpoint(
                        url: $"/swagger/{description.GroupName}/swagger.json",
                        name: description.GroupName.ToUpperInvariant()
                    );
                }
            });

        public static IApplicationBuilder ResolveSwagger(this IApplicationBuilder app, IHostEnvironment env, Action<SwaggerOptions> setupSwagger, Action<SwaggerUIOptions> setupSwaggerUI) {
            if (env.IsDevelopment()) {
                app.UseSwagger(setupSwagger);
                app.UseSwaggerUI(setupSwaggerUI);
            }

            return app;
        }

        public static IApplicationBuilder ResolveErrorHandling(this IApplicationBuilder app, IHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseExceptionHandler("/error");
            }

            return app;
        }

        #endregion
    }
}
