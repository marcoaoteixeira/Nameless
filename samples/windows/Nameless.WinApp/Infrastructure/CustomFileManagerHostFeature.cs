using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Nameless.Application;
using Nameless.Hosting;
using Nameless.IO;

namespace Nameless.WinApp.Infrastructure;

internal sealed class CustomFileManagerHostFeature : HostFeature {
    public override void Register(IHostApplicationBuilder builder, HostFeatureContext context) {
        builder.Services.TryAddSingleton<IFileManager>(
            provider => provider.GetRequiredService<IApplicationContext>().FileManager
        );
    }
}
