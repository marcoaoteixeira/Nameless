using System.Net.Security;
using Microsoft.Extensions.Logging;

namespace Nameless.Caching.Redis.Internals;
/// <summary>
/// To know why, see: <a href="https://learn.microsoft.com/en-us/dotnet/core/extensions/high-performance-logging">High-performance logging in .NET</a>
/// </summary>
internal static class LoggerExtension {
    private static readonly Action<ILogger,
        Exception?> CantUseEvictionCallbackHandler
        = LoggerMessage.Define(logLevel: LogLevel.Information,
                               eventId: default,
                               formatString: "It's not possible to configure eviction callbacks for RedisCache.",
                               options: null);

    internal static void CantUseEvictionCallback(this ILogger self)
        => CantUseEvictionCallbackHandler(self, null /* exception */);

    private static readonly Action<ILogger,
        SslPolicyErrors,
        Exception?> CertificateValidationErrorHandler
        = LoggerMessage.Define<SslPolicyErrors>(logLevel: LogLevel.Warning,
                                                eventId: default,
                                                formatString: "Certificate error: {sslPolicyErrors}",
                                                options: null);

    internal static void CertificateValidationError(this ILogger self, SslPolicyErrors sslPolicyErrors)
        => CertificateValidationErrorHandler(self, sslPolicyErrors, null /* exception */);
}
