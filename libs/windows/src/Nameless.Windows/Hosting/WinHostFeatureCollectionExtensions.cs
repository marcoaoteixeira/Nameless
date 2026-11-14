using Nameless.Hosting;
using Nameless.Hosting.Features.Application;
using Nameless.Hosting.Features.Common;
using Nameless.Hosting.Features.FileManager;
using Nameless.Hosting.Features.Logging;
using Nameless.Hosting.Features.Lucene;
using Nameless.Hosting.Features.Mediator;
using Nameless.Hosting.Features.Resilience;
using Nameless.Hosting.Features.Validation;
using Nameless.Windows.Hosting.Features.AppConfiguration;
using Nameless.Windows.Hosting.Features.Bootstrap;
using Nameless.Windows.Hosting.Features.Dialogs.FileSystem;
using Nameless.Windows.Hosting.Features.Dialogs.Message;
using Nameless.Windows.Hosting.Features.Documents;
using Nameless.Windows.Hosting.Features.Events;
using Nameless.Windows.Hosting.Features.Localization;
using Nameless.Windows.Hosting.Features.Mvvm;
using Nameless.Windows.Hosting.Features.Navigation;
using Nameless.Windows.Hosting.Features.Office;
using Nameless.Windows.Hosting.Features.TaskRunner;
using Nameless.Windows.Hosting.Features.UI;
using Nameless.Windows.Notification.SnackBar;
using Nameless.Workers;

namespace Nameless.Windows.Hosting;

internal static class WinHostFeatureCollectionExtensions {
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
                   .Add(new TranslatorHostFeature());
        }
    }
}
