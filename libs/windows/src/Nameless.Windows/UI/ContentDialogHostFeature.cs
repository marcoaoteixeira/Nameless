using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Nameless.Hosting;
using Wpf.Ui;

namespace Nameless.Windows.UI;

/// <summary>
///     Content Dialog Host Feature
/// </summary>
public class ContentDialogHostFeature : HostFeature {
    /// <inheritdoc/>
    public override void Register(IHostApplicationBuilder builder, HostFeatureContext context) {
        builder.Services.TryAddSingleton<IContentDialogService, ContentDialogService>();
    }
}