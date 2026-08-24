using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Nameless.Helpers;
using Nameless.Resilience;

namespace Nameless.IO.Monitoring;

/// <summary>
///     Current implementation of <see cref="IFileMonitorProvider"/>.
/// </summary>
public class FileMonitorProvider : IFileMonitorProvider {
    private readonly IServiceProvider _provider;

    /// <summary>
    ///     Initializes a new instance
    ///     of <see cref="FileMonitorProvider"/> class.
    /// </summary>
    /// <param name="provider">
    ///     The current service provider.
    /// </param>
    public FileMonitorProvider(IServiceProvider provider) {
        _provider = provider;
    }

    /// <inheritdoc />
    public IFileMonitor Create(Action<FileMonitorOptions> options) {
        var opts = ActionHelper.FromDelegate(options);

        return new FileMonitor(
            fileProvider: new PhysicalFileProvider(opts.RootDirectory),
            retryPipelineFactory: opts.EnableRetry
                ? _provider.GetRequiredService<IRetryPipelineFactory>()
                : NullRetryPipelineFactory.Instance,
            options: Options.Create(opts),
            logger: _provider.GetLogger<FileMonitor>()
        );
    }
}