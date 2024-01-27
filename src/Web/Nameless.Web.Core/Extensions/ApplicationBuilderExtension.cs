using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Asp.Versioning.ApiExplorer;
using Autofac.Extensions.DependencyInjection;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Nameless.FluentValidation;
using Nameless.Web.Infrastructure;
using Nameless.Web.Middlewares;

namespace Nameless.Web {
    public static class ApplicationBuilderExtension {
        #region Public Static Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="self"></param>
        /// <param name="assemblies"></param>
        /// <returns></returns>
        public static IApplicationBuilder ResolveMinimalEndpoints(this IApplicationBuilder self, params Assembly[] assemblies)
            => self.UseEndpoints(setup => {
                var endpointTypes = assemblies
                    .SelectMany(_ => _.ExportedTypes)
                    .Where(_ => typeof(IMinimalEndpoint).IsAssignableFrom(_))
                    .ToArray();

                if (endpointTypes.Length == 0) { return; }

                var loggerFactory = self.ApplicationServices.GetService<ILoggerFactory>();
                var logger = loggerFactory is not null
                    ? loggerFactory.CreateLogger(typeof(ApplicationBuilderExtension))
                    : NullLogger.Instance;

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

        public static IApplicationBuilder ResolveCors(this IApplicationBuilder self)
            => self
                .UseCors(configure
                    => configure
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                );

        public static IApplicationBuilder ResolveRouting(this IApplicationBuilder self)
            => self
                .UseRouting();

        public static IApplicationBuilder ResolveAuth(this IApplicationBuilder self)
            => self
                .UseAuthorization()
                .UseAuthentication()
                .UseMiddleware<JwtAuthorizationMiddleware>();

        public static IApplicationBuilder ResolveHealthChecks(this IApplicationBuilder self)
            => self
                .UseHealthChecks("/healthz");

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

        public static IApplicationBuilder ResolveHttpSecurity(this IApplicationBuilder self, IHostEnvironment env) {
            if (!env.IsDevelopment()) {
                // The default HSTS value is 30 days. You may want to change
                // this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                self.UseHsts();
            }
            self.UseHttpsRedirection();

            return self;
        }

        public static IApplicationBuilder ResolveErrorHandling(this IApplicationBuilder self) {
            self.UseExceptionHandler(configure =>
                configure.Run(async ctx => {
                    if (await TryHandleValidationException(ctx)) {
                        return;
                    }

                    await Results.Problem().ExecuteAsync(ctx);
                })
            );

            return self;
        }

        public static IApplicationBuilder ResolveAutofac(this IApplicationBuilder self, IHostApplicationLifetime lifetime) {
            // Tear down the composition root and free all resources.
            var container = self.ApplicationServices.GetAutofacRoot();
            lifetime.ApplicationStopped.Register(container.Dispose);

            return self;
        }

        #endregion

        #region Private Static Methods

        private static bool TryCreateEndpoint(Type type, ILogger logger, [NotNullWhen(returnValue: true)] out IMinimalEndpoint? endpoint) {
            endpoint = null;

            try { endpoint = Activator.CreateInstance(type) as IMinimalEndpoint; }
            catch (Exception ex) { logger.LogError(ex, "{Message}", ex.Message); }

            return endpoint is not null;
        }

        private static TException? GetExceptionFromHttpContext<TException>(HttpContext httpContext)
            where TException : Exception {
            var feature = httpContext.Features.Get<IExceptionHandlerPathFeature>();
            return feature is not null
                ? feature.Error is TException exception
                    ? exception
                    : null
                : null;
        }

        private static async Task<bool> TryHandleValidationException(HttpContext ctx) {
            var ex = GetExceptionFromHttpContext<ValidationException>(ctx);
            if (ex is not null) {
                await Results.ValidationProblem(
                    errors: ex.ToDictionary(),
                    detail: ex.Message
                ).ExecuteAsync(ctx);

                return true;
            }
            return false;
        }

        #endregion
    }
}
