using System.Diagnostics.CodeAnalysis;
using Asp.Versioning.ApiExplorer;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nameless.Validation.Abstractions;
using Nameless.Web.Filters;
using Nameless.Web.Infrastructure;
using Nameless.Web.Middlewares;

namespace Nameless.Web {
    public static class ApplicationBuilderExtension {
        #region Public Static Methods

        public static IApplicationBuilder ResolveJwtAuth(this IApplicationBuilder self)
            => self.UseMiddleware<JwtAuthorizationMiddleware>()
                   .UseAuthorization()
                   .UseAuthentication();

        public static IApplicationBuilder ResolveAutofac(this IApplicationBuilder self, IHostApplicationLifetime lifetime) {
            // Tear down the composition root and free all resources
            // when the application stops.
            var container = self.ApplicationServices.GetAutofacRoot();
            lifetime.ApplicationStopped.Register(container.Dispose);

            return self;
        }

        public static IApplicationBuilder ResolveCors(this IApplicationBuilder self, Action<CorsPolicyBuilder>? configure = null) {
            return self.UseCors(configure ?? DefaultPolicy);

            static void DefaultPolicy(CorsPolicyBuilder builder)
                => builder.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
        }

        /// <summary>
        /// Resolves the endpoint service.
        /// This call must be preceded by a call to <see cref="ResolveRouting"/>.
        /// </summary>
        /// <param name="self">The <see cref="IApplicationBuilder"/> instance.</param>
        /// <param name="configure">The configure action for endpoints. If not provided, will try to resolve all registered minimal endpoints (<see cref="IMinimalEndpoint"/>) implementations.</param>
        /// <returns>The <see cref="IApplicationBuilder"/> instance.</returns>
        public static IApplicationBuilder ResolveEndpoints(this IApplicationBuilder self, Action<IEndpointRouteBuilder>? configure = null)
            => self.UseEndpoints(configure ?? (_ => { }));

        public static IApplicationBuilder ResolveErrorHandling(this IApplicationBuilder self, bool enableValidationExceptionTreatment = true) {
            return self.UseExceptionHandler(enableValidationExceptionTreatment ? ValidationExceptionTreatment : _ => { });

            static void ValidationExceptionTreatment(IApplicationBuilder builder)
                => builder.Run(ctx => TryHandleValidationException(ctx, out var result)
                                   ? result.ExecuteAsync(ctx)
                                   : Results.Problem().ExecuteAsync(ctx));
        }

        public static IApplicationBuilder ResolveHealthChecks(this IApplicationBuilder self, string path = "/healthz", int port = 80, HealthCheckOptions? options = null)
            => self.UseHealthChecks(path, port, options ?? new HealthCheckOptions());

        public static IApplicationBuilder ResolveHttpSecurity(this IApplicationBuilder self, IHostEnvironment env) {
            if (!env.IsDevelopment()) {
                // The default HSTS value is 30 days. You may want to change
                // this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                self.UseHsts();
            }

            return self.UseHttpsRedirection();
        }

        /// <summary>
        /// Resolves the endpoint service.
        /// This call must be preceded by a call to <see cref="ResolveRouting"/>.
        /// </summary>
        /// <param name="self">The <see cref="IApplicationBuilder"/> instance.</param>
        /// <returns>The <see cref="IApplicationBuilder"/> instance.</returns>
        public static IApplicationBuilder ResolveMinimalEndpoints(this IApplicationBuilder self)
            => self.UseEndpoints(builder => {
                var endpoints = builder.ServiceProvider
                                       .GetRequiredService<IEnumerable<IMinimalEndpoint>>();

                foreach (var endpoint in endpoints) {
                    endpoint
                        .Map(builder)
                        .WithOpenApi()

                        .WithName(endpoint.Name)
                        .WithSummary(endpoint.Summary)
                        .WithDescription(endpoint.Description)

                        .WithApiVersionSet(builder.NewApiVersionSet(endpoint.Group)
                                                  .Build())

                        .HasApiVersion(endpoint.Version)

                        .AddEndpointFilter(new ValidateEndpointFilter());
                }
            });

        /// <summary>
        /// Resolves the routing service. This call must be followed by a call to <see cref="ResolveEndpoints"/>.
        /// </summary>
        /// <param name="self">The application builder instance.</param>
        /// <returns>The application builder instance.</returns>
        public static IApplicationBuilder ResolveRouting(this IApplicationBuilder self)
            => self.UseRouting();

        public static IApplicationBuilder ResolveSwagger(this IApplicationBuilder self, IApiVersionDescriptionProvider versioning)
            => self.UseSwagger()
                   .UseSwaggerUI(setup => {
                       foreach (var description in versioning.ApiVersionDescriptions) {
                           setup.SwaggerEndpoint(
                               url: $"/swagger/{description.GroupName}/swagger.json",
                               name: description.GroupName.ToUpperInvariant()
                           );
                       }
                   });

        #endregion

        #region Private Static Methods

        private static TException? GetExceptionFromHttpContext<TException>(HttpContext httpContext)
            where TException : Exception {
            var feature = httpContext.Features.Get<IExceptionHandlerPathFeature>();
            return feature?.Error as TException;
        }

        private static bool TryHandleValidationException(HttpContext ctx, [NotNullWhen(returnValue: true)] out IResult? result) {
            result = null;

            var ex = GetExceptionFromHttpContext<ValidationException>(ctx);
            if (ex is null) { return false; }

            result = Results.ValidationProblem(errors: ex.Result.ToDictionary(),
                                               detail: ex.Message);

            return true;
        }

        #endregion
    }
}
