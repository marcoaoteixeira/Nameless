using System.Net.Security;
using System.Security.Authentication;
#if NET6_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif

namespace Nameless.ProducerConsumer.RabbitMQ.Options;

public sealed class SslOptions {
    public static SslOptions Default => new();

    public bool Enabled { get; set; }

    public string? ServerName { get; set; }

    public SslProtocols Protocol { get; set; }

    public SslPolicyErrors PolicyError { get; set; }

#if NET6_0_OR_GREATER
        [MemberNotNullWhen(returnValue: true, members: [nameof(ServerName)])]
#endif
    internal bool IsAvailable
        => Enabled &&
           !string.IsNullOrWhiteSpace(ServerName) &&
           Protocol != SslProtocols.None;
}