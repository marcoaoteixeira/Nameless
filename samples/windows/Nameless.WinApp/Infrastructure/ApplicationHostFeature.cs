using Nameless.Application;
using Nameless.Hosting;
using Nameless.Hosting.Features.FileManager;
using Nameless.Hosting.Features.Logging;
using Nameless.WinApp.Data;
using Nameless.Windows.Localization;
using Serilog;
using Serilog.Formatting.Compact;

namespace Nameless.WinApp.Infrastructure;

internal static class ApplicationHostFeature {
    public static void Configure(HostFeatureCollection features) {
        ConfigureTranslatorHostFeature(features);
        ConfigureEntityFrameworkCoreHostFeature(features);
        ConfigureLoggingFeature(features);
        ConfigureFileManagerFeature(features);
        ConfigureSystemUpdateFeature(features);
    }

    private static void ConfigureSystemUpdateFeature(HostFeatureCollection features) {
        features.Add(new SystemUpdateHostFeature());
    }

    private static void ConfigureFileManagerFeature(HostFeatureCollection features) {
        features.Replace<PhysicalFileManagerHostFeature>(
            new CustomFileManagerHostFeature()
        );
    }

    private static void ConfigureTranslatorHostFeature(HostFeatureCollection features) {
        features.Get<TranslatorHostFeature>().Configure(
            opts => opts.WithTranslator<JsonTranslator>()
        );
    }

    private static void ConfigureEntityFrameworkCoreHostFeature(HostFeatureCollection features) {
        var feature = new EntityFrameworkCoreHostFeature<AppDbContext>();

        feature.Configure(opts => opts.WithDbContextConfiguration((provider, ctx) => {
            var applicationContext = provider.GetRequiredService<IApplicationContext>();
            var databaseFile = applicationContext.FileManager.GetFile(
                relativePath: SysPath.Combine(
                    applicationContext.FileManager.GetDatabaseDirectory().Name,
                    Constants.Database.FileName
                )
            );
            var connStr = string.Format(Constants.Database.ConnStrPattern, databaseFile.Path);

            ctx.UseSqlite(connStr);
        }));

        features.Add(feature);
    }

    private static void ConfigureLoggingFeature(HostFeatureCollection feature) {
        feature.Get<SerilogHostFeature>().Configure(opts => opts.WithSinkConfiguration((_, sink) => sink.File(
            path: SysPath.Combine(Constants.Logging.Directory, Constants.Logging.FileNamePattern),
            rollingInterval: RollingInterval.Day,
            fileSizeLimitBytes: 16_777_216,
            retainedFileCountLimit: 3,
            formatter: new CompactJsonFormatter()
        )));
    }
}
