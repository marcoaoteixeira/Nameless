using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Nameless.Hosting;

namespace Nameless.Windows.Events;

/// <summary>
///     Event Publisher Host Feature
/// </summary>
public class EventPublisherHostFeature : HostFeature {
    /// <inheritdoc/>
    public override void Register(IHostApplicationBuilder builder, HostFeatureContext context) {
        builder.Services.TryAddSingleton<IEventPublisher>(
            new InProcessEventPublisher(new WeakReferenceMessenger())
        );
    }
}