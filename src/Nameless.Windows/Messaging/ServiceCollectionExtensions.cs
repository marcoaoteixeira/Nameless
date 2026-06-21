using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Nameless.Windows.Messaging.Impl;

namespace Nameless.Windows.Messaging;

public static class ServiceCollectionExtensions {
    extension(IServiceCollection self) {
        public IServiceCollection RegisterMessenger() {
            self.TryAddSingleton<IMessenger>(
                new Messenger(new WeakReferenceMessenger())
            );

            return self;
        }
    }
}
