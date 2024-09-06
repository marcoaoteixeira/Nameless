using System.Net.Security;
using Microsoft.Extensions.Logging;

namespace Nameless.Caching.Redis;
/// <summary>
/// To know why, see: <a href="https://learn.microsoft.com/en-us/dotnet/core/extensions/high-performance-logging">High-performance logging in .NET</a>
/// </summary>
internal static class LoggerHelper {
    internal static readonly Action<ILogger, Exception?> CantUseEvictionCallback = LoggerMessage.Define(
        logLevel: LogLevel.Information,
        eventId: default,
        formatString: "It's not possible to configure eviction callbacks for RedisCache.",
        options: null
    );

    internal static readonly Action<ILogger, SslPolicyErrors, Exception?> OnCertificateValidationFailure = LoggerMessage.Define<SslPolicyErrors>(
        logLevel: LogLevel.Error,
        eventId: default,
        formatString: "Certificate error: {sslPolicyErrors}",
        options: null
    );
}
