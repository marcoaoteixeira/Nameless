using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nameless.Contracts;

namespace Nameless.Windows.UI;

/// <summary>
///     Window Factory
/// </summary>
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
        _provider = Throws.When.Null(provider);
        _logger = Throws.When.Null(logger);
    }

    /// <inheritdoc />
    public TWindow Create<TWindow>() where TWindow : IWindow {
        try { return _provider.GetRequiredService<TWindow>(); }
        catch (Exception ex) {
            CommonLog.Error(_logger, $"Can't initialize window of type {typeof(TWindow)}", ex, GetType().Tag);

            throw;
        }
    }
}