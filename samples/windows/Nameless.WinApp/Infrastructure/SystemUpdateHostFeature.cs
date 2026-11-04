using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Nameless.Application;
using Nameless.Hosting;
using Nameless.WinApp.Infrastructure.GitHub;

namespace Nameless.WinApp.Infrastructure;

/// <summary>
///     Registers the GitHub Releases client and the environment seam the
///     self-update feature depends on (plan §3.2).
/// </summary>
internal sealed class SystemUpdateHostFeature : HostFeature {
    public override void Register(IHostApplicationBuilder builder, HostFeatureContext context) {
        builder.Services.AddHttpClient<IGitHubReleaseClient, GitHubReleaseClient>((provider, client) => {
            var applicationContext = provider.GetRequiredService<IApplicationContext>();

            client.BaseAddress = new Uri("https://api.github.com/");
            client.Timeout = TimeSpan.FromSeconds(15);
            client.DefaultRequestHeaders.Add("Accept", "application/vnd.github+json");
            client.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2022-11-28");
            client.DefaultRequestHeaders.Add("User-Agent", $"Nameless.WinApp/{applicationContext.Version}");
        });

        builder.Services.TryAddSingleton<ISystemUpdateEnvironment, SystemUpdateEnvironment>();
        builder.Services.TryAddSingleton<IApplicationRelauncher, ApplicationRelauncher>();
    }
}
