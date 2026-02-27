using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Nameless.Infrastructure;
using Nameless.IO.FileSystem;
using Nameless.Lucene.Infrastructure.Implementations;
using Index = Nameless.Lucene.Infrastructure.Implementations.Index;

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

        services.RegisterLucene(
            registration: _ => { },
            configure: _ => { }
        );

        return services.BuildServiceProvider();
    }
}
