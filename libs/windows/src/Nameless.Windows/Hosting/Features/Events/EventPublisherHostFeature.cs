using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Nameless.Hosting;
using Nameless.Windows.Events;

namespace Nameless.Windows.Hosting.Features.Events;

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