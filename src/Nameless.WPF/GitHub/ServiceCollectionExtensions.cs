using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nameless.WPF.GitHub.Impl;

namespace Nameless.WPF.GitHub;

/// <summary>
///     <see cref="IServiceCollection"/> extension methods.
/// </summary>
public static class ServiceCollectionExtensions {
    /// <param name="self">
    ///     The current <see cref="IServiceCollection"/>.
    /// </param>
    extension(IServiceCollection self) {
        /// <summary>
        ///     Register GitHub HTTP client.
        /// </summary>
        /// <param name="configure">
        ///     The configuration action.
        /// </param>
        /// <returns>
        ///     The current <see cref="IServiceCollection"/> so other actions
        ///     can be chained.
        /// </returns>
        public IServiceCollection RegisterGitHubHttpClient(Action<GitHubOptions>? configure = null) {
            return self.Configure(configure ?? (_ => { }))
                       .InnerRegisterGitHubHttpClient();
        }

        /// <summary>
        ///     Register GitHub HTTP client.
        /// </summary>
        /// <param name="configuration">
        ///     The configuration.
        /// </param>
        /// <returns>
        ///     The current <see cref="IServiceCollection"/> so other actions
        ///     can be chained.
        /// </returns>
        public IServiceCollection RegisterGitHubHttpClient(IConfiguration configuration) {
            var section = configuration.GetSection<GitHubOptions>();

            return self.Configure<GitHubOptions>(section)
                       .InnerRegisterGitHubHttpClient();
        }

        private IServiceCollection InnerRegisterGitHubHttpClient() {
            self.AddHttpClient<IGitHubHttpClient, GitHubHttpClient>((provider, client) => {
                var opts = provider.GetOptions<GitHubOptions>().Value;

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github+json"));
                client.DefaultRequestHeaders.Add("X-GitHub-Api-Version", [opts.ApiVersion]);

                if (string.IsNullOrWhiteSpace(opts.ApiBaseUrl)) {
                    throw new InvalidOperationException("Missing GitHub API URL.");
                }

                client.BaseAddress = new Uri(opts.ApiBaseUrl);
            });

            return self;
        }
    }
}
