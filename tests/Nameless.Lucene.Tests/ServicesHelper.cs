using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Nameless.Infrastructure;
using Nameless.IO.FileSystem;

namespace Nameless.Lucene;
internal static class ServicesHelper {
    internal static string RootDirectoryPath => typeof(ServicesHelper).Assembly.GetDirectoryPath();
    internal static string IndexesDirectoryName => "Lucene";

    internal static ServiceProvider CreateContainer() {
        var services = new ServiceCollection();

        services.TryAddSingleton((ILogger<Index>)NullLogger<Index>.Instance);
        services.TryAddSingleton((ILogger<IndexProvider>)NullLogger<IndexProvider>.Instance);

        services
            .RegisterApplicationContext()
            .RegisterFileSystem(opts => {
                opts.Root = RootDirectoryPath;
            });

        services.RegisterLucene(opts => {
            opts.SetDirectoryName(IndexesDirectoryName);
        });

        return services.BuildServiceProvider();
    }
}
