using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;

namespace Nameless.GitHub;

internal static class LoggerExtensions {
    extension(ILogger<GitHubHttpClient> self) {
        internal void Failure(string actionName, Exception ex) {
            Log.Failure(
                logger: self,
                tag: "GITHUB_HTTP_CLIENT",
                actionName,
                exception: ex
            );
        }

        internal void Warning(string actionName, string reason) {
            Log.Warning(
                logger: self,
                tag: "GITHUB_HTTP_CLIENT",
                actionName,
                reason
            );
        }
    }
}
