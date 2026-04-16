using System.Net.Http.Headers;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Nameless.Bootstrap;
using Nameless.Compression.Zip;
using Nameless.Infrastructure;
using Nameless.Mediator;
using Nameless.Validation.FluentValidation;
using Nameless.WPF.Behaviors;
using Nameless.WPF.Client.Lucene;
using Nameless.WPF.Client.Sqlite;
using Nameless.WPF.Configuration;
using Nameless.WPF.Dialogs.FileSystem;
using Nameless.WPF.Dialogs.Message;
using Nameless.WPF.DisasterRecovery;
using Nameless.WPF.GitHub;
using Nameless.WPF.Messaging;
using Nameless.WPF.Mvvm;
using Nameless.WPF.Navigation;
using Nameless.WPF.SnackBar;
using Nameless.WPF.TaskRunner;
using Nameless.WPF.Windows;
using NLog.Extensions.Logging;
using Wpf.Ui;

namespace Nameless.WPF.Client;

/// <summary>
///     <see cref="IServiceCollection"/> extension methods.
/// </summary>
public static class ServiceCollectionExtensions {
    extension(IServiceCollection self) {
        public IServiceCollection RegisterWPF(Assembly[] supportAssemblies, IConfiguration configuration) {
            // The Main
            self.AddLogging(logging => {
                logging.ClearProviders();
                logging.AddNLog();
            });
            self.TryAddSingleton(TimeProvider.System);
            self.ConfigureHttpClientDefaults(builder => {
                builder.ConfigureHttpClient((provider, client) => {
                    var applicationContext = provider.GetRequiredService<IApplicationContext>();
                    var productHeaderValue = new ProductHeaderValue(applicationContext.ApplicationName, applicationContext.Version);
                    var userAgent = new ProductInfoHeaderValue(productHeaderValue);

                    client.DefaultRequestHeaders.UserAgent.Add(userAgent);
                });
            });

            // From Third-party
            self.RegisterApplicationContext(configuration);
            self.RegisterMediator(registration => {
                registration.IncludeAssemblies(supportAssemblies);
                registration.RegisterRequestPipelineBehavior(typeof(PerformanceRequestPipelineBehavior<,>));
                registration.RegisterRequestPipelineBehavior(typeof(ValidateRequestPipelineBehavior<,>));
            });
            self.RegisterValidation(
                registration =>  registration.IncludeAssemblies(supportAssemblies)
            );

            // From Core
            self.TryAddSingleton<IContentDialogService, ContentDialogService>();
            self.RegisterDisasterRecovery(
                registration => registration.IncludeAssemblies(supportAssemblies)
            );
            self.RegisterAppConfigurationManager();
            self.RegisterEntityFrameworkCoreForSqlite(
                registration => registration.IncludeAssemblies(supportAssemblies)
            );
            self.RegisterBootstrap(
                registration => registration.IncludeAssemblies(supportAssemblies),
                configuration
            );
            self.RegisterMessenger();
            self.RegisterGitHubHttpClient();
            self.RegisterFileSystemDialog();
            self.RegisterLucene(supportAssemblies);
            self.RegisterMessageDialog();
            self.RegisterNavigation(
                registration =>  registration.IncludeAssemblies(supportAssemblies)
            );
            self.RegisterSnackBar();
            self.RegisterTaskRunner();
            self.RegisterViewModels(
                registration => registration.IncludeAssemblies(supportAssemblies)
            );
            self.RegisterWindowFactory(
                registration => registration.IncludeAssemblies(supportAssemblies)
            );
            self.RegisterZipCompression();

            return self;
        }
    }
}
