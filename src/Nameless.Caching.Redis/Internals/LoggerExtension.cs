using System.Net.Security;
using Microsoft.Extensions.Logging;

namespace Nameless.Caching.Redis.Internals;
/// <summary>
/// To know why, see: <a href="https://learn.microsoft.com/en-us/dotnet/core/extensions/high-performance-logging">High-performance logging in .NET</a>
/// </summary>
internal static class LoggerExtension {
    private static readonly Action<ILogger,
        Exception?> _cantUseEvictionCallback
        = LoggerMessage.Define(logLevel: LogLevel.Information,
                               eventId: default,
                               formatString: "It's not possible to configure eviction callbacks for RedisCache.",
                               options: null);

    private static readonly Action<ILogger,
        SslPolicyErrors,
        Exception?> _certificateValidationFailure
        = LoggerMessage.Define<SslPolicyErrors>(logLevel: LogLevel.Warning,
                                                eventId: default,
                                                formatString: "Certificate error: {sslPolicyErrors}",
                                                options: null);

    internal static void CantUseEvictionCallback(this ILogger self)
        => _cantUseEvictionCallback(self, null /* exception */);

    internal static void CertificateValidationFailure(this ILogger self, SslPolicyErrors sslPolicyErrors)
        => _certificateValidationFailure(self, sslPolicyErrors, null /* exception */);
}
