using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Asp.Versioning.ApiExplorer;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nameless.FluentValidation;
using Nameless.Web.Infrastructure;
using Nameless.Web.Middlewares;
using FluentValidationException = FluentValidation.ValidationException;

namespace Nameless.Web {
    public static class ApplicationBuilderExtension {
        #region Public Static Methods

        public static IApplicationBuilder ResolveJwtAuth(this IApplicationBuilder self)
            => self
                .UseMiddleware<JwtAuthorizationMiddleware>()
                .UseAuthorization()
                .UseAuthentication();

        public static IApplicationBuilder ResolveAutofac(this IApplicationBuilder self, IHostApplicationLifetime lifetime) {
            // Tear down the composition root and free all resources.
            var container = self.ApplicationServices.GetAutofacRoot();
            lifetime.ApplicationStopped.Register(container.Dispose);

            return self;
        }

        public static IApplicationBuilder ResolveCors(this IApplicationBuilder self)
            => self
                .UseCors(configure => configure.AllowAnyOrigin()
                                               .AllowAnyMethod()
                                               .AllowAnyHeader());

        public static IApplicationBuilder ResolveErrorHandling(this IApplicationBuilder self) {
            self.UseExceptionHandler(configure =>
                configure.Run(async ctx => {
                    if (TryHandleFluentValidationException(ctx, out var result)) {
                        await result.ExecuteAsync(ctx);

                        return;
                    }

                    await Results.Problem().ExecuteAsync(ctx);
                })
            );

            return self;
        }

        public static IApplicationBuilder ResolveHealthChecks(this IApplicationBuilder self)
            => self
                .UseHealthChecks("/healthz");

        public static IApplicationBuilder ResolveHttpSecurity(this IApplicationBuilder self, IHostEnvironment env) {
            if (!env.IsDevelopment()) {
                // The default HSTS value is 30 days. You may want to change
                // this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                self.UseHsts();
            }
            self.UseHttpsRedirection();

            return self;
        }

        /// <summary>
        /// Resolves all defined minimal endpoints.
        /// </summary>
        /// <param name="self">The application builder.</param>
        /// <param name="assemblies">The assemblies for scanning.</param>
        /// <returns></returns>
        public static IApplicationBuilder ResolveMinimalEndpoints(this IApplicationBuilder self, params Assembly[] assemblies)
            => self.UseEndpoints(setup => {
                var endpointTypes = assemblies
                    .SelectMany(type => type.ExportedTypes)
                    .Where(type => type is { IsInterface: false, IsAbstract: false } &&
                                   typeof(IMinimalEndpoint).IsAssignableFrom(type))
                    .ToArray();

                if (endpointTypes.Length == 0) { return; }

                var logger = self.ApplicationServices
                    .GetLogger(typeof(ApplicationBuilderExtension));

                foreach (var endpointType in endpointTypes) {
                    if (TryCreateEndpoint(endpointType, logger, out var endpoint)) {
                        endpoint
                            .Map(setup)
                            .WithOpenApi()

                            .WithName(endpoint.Name)
                            .WithSummary(endpoint.Summary)
                            .WithDescription(endpoint.Description)

                            .WithApiVersionSet(setup.NewApiVersionSet(endpoint.Group).Build())

                            .HasApiVersion(endpoint.Version);
                    }
                }
            });

        public static IApplicationBuilder ResolveRouting(this IApplicationBuilder self)
            => self
                .UseRouting();

        public static IApplicationBuilder ResolveSwagger(this IApplicationBuilder self, IApiVersionDescriptionProvider versioning)
            => self
                .UseSwagger()
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

        private static bool TryCreateEndpoint(Type type, ILogger logger, [NotNullWhen(returnValue: true)] out IMinimalEndpoint? endpoint) {
            endpoint = null;

            try { endpoint = Activator.CreateInstance(type) as IMinimalEndpoint; }
            catch (Exception ex) { logger.LogError(ex, "Error while trying to create minimal endpoint."); }

            return endpoint is not null;
        }

        private static TException? GetExceptionFromHttpContext<TException>(HttpContext httpContext)
            where TException : Exception {
            var feature = httpContext.Features.Get<IExceptionHandlerPathFeature>();
            return feature?.Error as TException;
        }

        private static bool TryHandleFluentValidationException(HttpContext ctx, [NotNullWhen(returnValue: true)]out IResult? result) {
            result = null;

            var ex = GetExceptionFromHttpContext<FluentValidationException>(ctx);
            if (ex is null) { return false; }

            result = Results.ValidationProblem(
                errors: ex.ToDictionary(),
                detail: ex.Message
            );

            return true;
        }

        #endregion
    }
}
