using Nameless.Application;
using Nameless.Hosting;
using Nameless.Hosting.Features;
using Nameless.IO.Physical;
using Nameless.Logging.Serilog;
using Nameless.Lucene;
using Nameless.Mediator;
using Nameless.Resilience;
using Nameless.Validation.FluentValidation;
using Nameless.Windows.AppConfiguration;
using Nameless.Windows.Bootstrap;
using Nameless.Windows.Dialogs.FileSystem;
using Nameless.Windows.Dialogs.Message;
using Nameless.Windows.Documents;
using Nameless.Windows.Events;
using Nameless.Windows.Localization;
using Nameless.Windows.Mvvm;
using Nameless.Windows.Navigation;
using Nameless.Windows.Notification.SnackBar;
using Nameless.Windows.Office;
using Nameless.Windows.TaskRunner;
using Nameless.Windows.UI;
using Nameless.Workers;

namespace Nameless.Windows.Hosting;

internal static class AppHostFeatureCollectionExtensions {
    extension(HostFeatureCollection self) {
        /// <summary>
        ///     Creates a new collection with the built-in features.
        /// </summary>
        /// <remarks>
        ///     Features with middlewares come first, in pipeline order. Features
        ///     that only register services come last; their position does not
        ///     affect the request pipeline.
        /// </remarks>
        /// <returns>
        ///     A new <see cref="HostFeatureCollection"/> instance.
        /// </returns>
        public static HostFeatureCollection CreateDefault() {
            return new HostFeatureCollection()
                   .Add(new AppConfigurationHostFeature())
                   .Add(new ApplicationContextHostFeature())
                   .Add(new BootstrapperHostFeature())
                   .Add(new CommonHostFeature())
                   .Add(new ContentDialogHostFeature())
                   .Add(new DocumentServiceHostFeature())
                   .Add(new EventPublisherHostFeature())
                   .Add(new FileSystemDialogHostFeature())
                   .Add(new FluentValidationHostFeature())
                   .Add(new LuceneHostFeature())
                   .Add(new MediatorHostFeature())
                   .Add(new MessageDialogHostFeature())
                   .Add(new MvvmHostFeature())
                   .Add(new NavigationHostFeature())
                   .Add(new OfficeHostFeature())
                   .Add(new PeriodicWorkerHostFeature())
                   .Add(new PhysicalFileManagerHostFeature())
                   .Add(new ResilienceHostFeature())
                   .Add(new SerilogHostFeature())
                   .Add(new SnackBarHostFeature())
                   .Add(new TaskRunnerHostFeature())
                   .Add(new TranslatorHostFeature())
                   .Add(new WindowHostFeature());
        }

        /// <summary>
        ///     Creates a new collection with the minimal built-in features.
        /// </summary>
        /// <remarks>
        ///     Features with middlewares come first, in pipeline order. Features
        ///     that only register services come last; their position does not
        ///     affect the request pipeline.
        /// </remarks>
        /// <returns>
        ///     A new <see cref="HostFeatureCollection"/> instance.
        /// </returns>
        public static HostFeatureCollection CreateMinimal() {
            return new HostFeatureCollection()
                   .Add(new AppConfigurationHostFeature())
                   .Add(new ApplicationContextHostFeature())
                   .Add(new CommonHostFeature())
                   .Add(new FluentValidationHostFeature())
                   .Add(new MediatorHostFeature())
                   .Add(new PeriodicWorkerHostFeature())
                   .Add(new PhysicalFileManagerHostFeature())
                   .Add(new ResilienceHostFeature())
                   .Add(new SerilogHostFeature())
                   .Add(new TranslatorHostFeature())
                   .Add(new WindowHostFeature());
        }
    }
}
