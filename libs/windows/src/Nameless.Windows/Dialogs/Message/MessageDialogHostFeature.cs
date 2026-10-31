using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Nameless.Hosting;

namespace Nameless.Windows.Dialogs.Message;

/// <summary>
///     Message Dialog Host Feature
/// </summary>
public class MessageDialogHostFeature : HostFeature {
    /// <inheritdoc/>
    public override void Register(IHostApplicationBuilder builder, HostFeatureContext context) {
        builder.Services.TryAddSingleton<IMessageDialog, MessageDialog>();
    }
}
