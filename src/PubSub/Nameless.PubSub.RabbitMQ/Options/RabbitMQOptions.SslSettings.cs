#if NET8_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif
using System.Net.Security;
using System.Security.Authentication;

namespace Nameless.PubSub.RabbitMQ.Options;

public sealed class SslSettings {
    public bool Enabled { get; set; }

    public string ServerName { get; set; } = string.Empty;

    public SslProtocols Protocol { get; set; }

    public SslPolicyErrors AcceptablePolicyErrors { get; set; }

#if NET8_0_OR_GREATER
        [MemberNotNullWhen(returnValue: true, members: [nameof(ServerName)])]
#endif
    internal bool IsAvailable
        => Enabled &&
           !string.IsNullOrWhiteSpace(ServerName) &&
           Protocol != SslProtocols.None;
}