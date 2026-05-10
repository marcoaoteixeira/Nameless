using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Nameless.WPF.Hosting;

// Simple wrapper to not pollute generic HostBuilder with extensions.
public sealed class WinHostBuilder : IHostBuilder {
    private readonly IHostBuilder _current;

    public IDictionary<object, object> Properties => _current.Properties;

    public WinHostBuilder(IHostBuilder current) {
        _current = current;
    }

    public IHostBuilder ConfigureHostConfiguration(Action<IConfigurationBuilder> configureDelegate) {
        return _current.ConfigureHostConfiguration(configureDelegate);
    }

    public IHostBuilder ConfigureAppConfiguration(Action<HostBuilderContext, IConfigurationBuilder> configureDelegate) {
        return _current.ConfigureAppConfiguration(configureDelegate);
    }

    public IHostBuilder ConfigureServices(Action<HostBuilderContext, IServiceCollection> configureDelegate) {
        return _current.ConfigureServices(configureDelegate);
    }

    public IHostBuilder UseServiceProviderFactory<TContainerBuilder>(IServiceProviderFactory<TContainerBuilder> factory) where TContainerBuilder : notnull {
        return _current.UseServiceProviderFactory(factory);
    }

    public IHostBuilder UseServiceProviderFactory<TContainerBuilder>(Func<HostBuilderContext, IServiceProviderFactory<TContainerBuilder>> factory) where TContainerBuilder : notnull {
        return _current.UseServiceProviderFactory(factory);
    }

    public IHostBuilder ConfigureContainer<TContainerBuilder>(Action<HostBuilderContext, TContainerBuilder> configureDelegate) {
        return _current.ConfigureContainer(configureDelegate);
    }

    public IHost Build() {
        return _current.Build();
    }
}