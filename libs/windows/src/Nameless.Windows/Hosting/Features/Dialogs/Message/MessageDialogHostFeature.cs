using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Nameless.Hosting;
using Nameless.Windows.Dialogs.Message;

namespace Nameless.Windows.Hosting.Features.Dialogs.Message;

/// <summary>
///     Message Dialog Host Feature
/// </summary>
public class MessageDialogHostFeature : HostFeature {
    /// <inheritdoc/>
    public override void Register(IHostApplicationBuilder builder, HostFeatureContext context) {
        builder.Services.TryAddSingleton<IMessageDialog, MessageDialog>();
    }
}
