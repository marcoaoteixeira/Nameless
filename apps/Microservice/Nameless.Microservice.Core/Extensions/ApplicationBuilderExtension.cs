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

        public static IApplicationBuilder ResolveAuth(this IApplicationBuilder self)
            => self.UseAuthorization()
                .UseAuthentication()
                .UseJwtAuthorization();

        public static IApplicationBuilder ResolveCors(this IApplicationBuilder self)
            => self.UseCors(configure
                => configure
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());

        public static IApplicationBuilder ResolveEndpoints(this IApplicationBuilder self)
            => ResolveEndpoints(self, setup => {
                var endpoints = self.ApplicationServices.GetServices<IEndpoint>();
                foreach (var endpoint in endpoints) {
                    endpoint.Map(setup);
                }
            });

        public static IApplicationBuilder ResolveEndpoints(this IApplicationBuilder self, Action<IEndpointRouteBuilder> setup)
            => self.UseEndpoints(setup);

        public static IApplicationBuilder ResolveHealthChecks(this IApplicationBuilder self)
            => self.UseHealthChecks("/healthz");

        public static IApplicationBuilder ResolveHealthChecks(this IApplicationBuilder self, PathString path, int port, HealthCheckOptions options)
            => self.UseHealthChecks(path, port, options);

        public static IApplicationBuilder ResolveHttpSecurity(this IApplicationBuilder self, IHostEnvironment env) {
            if (!env.IsDevelopment()) {
                // The default HSTS value is 30 days. You may want to change
                // this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                self.UseHsts();
            }

            self.UseHttpsRedirection();

            return self;
        }

        public static IApplicationBuilder ResolveRouting(this IApplicationBuilder self)
            => self.UseRouting();

        public static IApplicationBuilder ResolveSwagger(this IApplicationBuilder self, IHostEnvironment env)
            => ResolveSwagger(self, env, setupSwagger => { }, setupSwaggerUI => { });

        public static IApplicationBuilder ResolveSwagger(this IApplicationBuilder self, IHostEnvironment env, IApiVersionDescriptionProvider versioning)
            => ResolveSwagger(self, env, setupSwagger => { }, setupSwaggerUI => {
                foreach (var description in versioning.ApiVersionDescriptions) {
                    setupSwaggerUI.SwaggerEndpoint(
                        url: $"/swagger/{description.GroupName}/swagger.json",
                        name: description.GroupName.ToUpperInvariant()
                    );
                }
            });

        public static IApplicationBuilder ResolveSwagger(this IApplicationBuilder self, IHostEnvironment env, Action<SwaggerOptions> setupSwagger, Action<SwaggerUIOptions> setupSwaggerUI) {
            if (env.IsDevelopment()) {
                self.UseSwagger(setupSwagger);
                self.UseSwaggerUI(setupSwaggerUI);
            }

            return self;
        }

        public static IApplicationBuilder ResolveErrorHandling(this IApplicationBuilder self, IHostEnvironment env) {
            if (env.IsDevelopment()) {
                self.UseExceptionHandler("/error");
            }

            return self;
        }

        #endregion
    }
}
