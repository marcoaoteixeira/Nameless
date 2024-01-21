using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Nameless.Web.Infrastructure;

namespace Nameless.Web {
    public static class ApplicationBuilderExtension {
        #region Public Static Methods

        public static IApplicationBuilder UseMinimalEndpoints(this IApplicationBuilder self, params Assembly[] assemblies)
            => self.UseEndpoints(setup => {
                var endpoints = assemblies
                    .SelectMany(_ => _.ExportedTypes)
                    .Where(_ => typeof(IMinimalEndpoint).IsAssignableFrom(_))
                    .ToArray();

                if (endpoints.Length == 0) { return; }

                var loggerFactory = self.ApplicationServices.GetService<ILoggerFactory>();
                ILogger logger = NullLogger.Instance;
                if (loggerFactory is not null) {
                    logger = loggerFactory.CreateLogger(typeof(ApplicationBuilderExtension));
                }

                foreach (var endpoint in endpoints) {
                    if (Create(endpoint, logger) is IMinimalEndpoint instance) {
                        instance.Map(setup)
                            .WithOpenApi()

                            .WithName(instance.Name)
                            .WithSummary(instance.Summary)
                            .WithDescription(instance.Description)

                            .WithApiVersionSet(setup.NewApiVersionSet(instance.Group).Build())

                            .HasApiVersion(instance.Version);
                    }
                }
            });

        #endregion

        #region Private Static Methods

        private static IMinimalEndpoint? Create(Type type, ILogger logger) {
            IMinimalEndpoint? result = null;

            try { result = Activator.CreateInstance(type) as IMinimalEndpoint; } catch (Exception ex) { logger.LogError(ex, "{Message}", ex.Message); }

            return result;
        }

        #endregion
    }
}
