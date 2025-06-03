using System.Diagnostics.CodeAnalysis;
using System.Net.Security;
using System.Security.Authentication;

namespace Nameless.ProducerConsumer.RabbitMQ.Options;

public sealed class SslSettings {
    public bool Enabled { get; set; }

    public string ServerName { get; set; } = string.Empty;

    public SslProtocols Protocol { get; set; }

    public SslPolicyErrors AcceptablePolicyErrors { get; set; }

    [MemberNotNullWhen(true, [nameof(ServerName)])]
    internal bool IsAvailable
        => Enabled &&
           !string.IsNullOrWhiteSpace(ServerName) &&
           Protocol != SslProtocols.None;
}