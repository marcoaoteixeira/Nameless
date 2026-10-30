using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Nameless.Testing.Tools.Mockers.Hosting;

/// <summary>
///     <see cref="IHostApplicationBuilder"/> mocker.
/// </summary>
public class HostApplicationBuilderMocker : Mocker<IHostApplicationBuilder> {
    /// <summary>
    ///     Mocks <see cref="IHostApplicationBuilder.Services"/> property.
    /// </summary>
    /// <param name="returnValue">
    ///     The <see cref="IServiceCollection"/> instance.
    /// </param>
    /// <returns>
    ///     The current <see cref="HostApplicationBuilderMocker"/> instance
    ///     so other actions can be chained.
    /// </returns>
    public HostApplicationBuilderMocker WithServices(IServiceCollection returnValue) {
        MockInstance
            .SetupGet(mock => mock.Services)
            .Returns(returnValue);

        return this;
    }

    /// <summary>
    ///     Mocks <see cref="IHostApplicationBuilder.Services"/> property.
    /// </summary>
    /// <param name="configure">
    ///     A delegate to configure <see cref="IServiceCollection"/> instance.
    /// </param>
    /// <returns>
    ///     The current <see cref="HostApplicationBuilderMocker"/> instance
    ///     so other actions can be chained.
    /// </returns>
    public HostApplicationBuilderMocker WithServices(Action<IServiceCollection> configure) {
        var services = new ServiceCollection();

        configure(services);

        MockInstance
            .SetupGet(mock => mock.Services)
            .Returns(services);

        return this;
    }
}
