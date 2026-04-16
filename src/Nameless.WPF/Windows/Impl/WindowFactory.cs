using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Nameless.WPF.Windows.Impl;

public class WindowFactory : IWindowFactory {
    private readonly IServiceProvider _provider;
    private readonly ILogger<WindowFactory> _logger;

    /// <summary>
    ///     Initializes a new instance of
    ///     the <see cref="WindowFactory"/> class.
    /// </summary>
    /// <param name="provider">
    ///     The service provider.
    /// </param>
    /// <param name="logger">
    ///     The logger.
    /// </param>
    public WindowFactory(IServiceProvider provider, ILogger<WindowFactory> logger) {
        _provider = provider;
        _logger = logger;
    }

    /// <inheritdoc />
    public TWindow Create<TWindow>() where TWindow : IWindow {
        try { return _provider.GetRequiredService<TWindow>(); }
        catch (Exception ex) {
            _logger.CreateWindowFailure(typeof(TWindow), ex);

            throw;
        }
    }
}