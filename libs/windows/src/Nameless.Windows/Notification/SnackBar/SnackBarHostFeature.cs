using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Nameless.Hosting;
using Wpf.Ui;

namespace Nameless.Windows.Notification.SnackBar;

/// <summary>
///     Snack Bar Host Feature
/// </summary>
public class SnackBarHostFeature : HostFeature {
    /// <inheritdoc/>
    public override void Register(IHostApplicationBuilder builder, HostFeatureContext context) {
        builder.Services.TryAddSingleton<ISnackbarService, SnackbarService>();
    }
}