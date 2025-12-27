using System.Net;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Http;

namespace Nameless.Web.Null;

/// <summary>
///     Null implementation of <see cref="ConnectionInfo"/> that does not store any connection information.
/// </summary>
public sealed class NullConnectionInfo : ConnectionInfo {
    public static ConnectionInfo Instance { get; } = new NullConnectionInfo();

    /// <inheritdoc />
    public override string Id {
        get => string.Empty;
        set => _ = value;
    }

    /// <inheritdoc />
    public override IPAddress? RemoteIpAddress {
        get => null;
        set => _ = value;
    }

    /// <inheritdoc />
    public override int RemotePort {
        get => 0;
        set => _ = value;
    }

    /// <inheritdoc />
    public override IPAddress? LocalIpAddress {
        get => null;
        set => _ = value;
    }

    /// <inheritdoc />
    public override int LocalPort {
        get => 0;
        set => _ = value;
    }

    /// <inheritdoc />
    public override X509Certificate2? ClientCertificate {
        get => null;
        set => _ = value;
    }

    static NullConnectionInfo() { }

    private NullConnectionInfo() { }

    /// <inheritdoc />
    public override Task<X509Certificate2?> GetClientCertificateAsync(CancellationToken cancellationToken = default) {
        return Task.FromResult<X509Certificate2?>(result: null);
    }
}