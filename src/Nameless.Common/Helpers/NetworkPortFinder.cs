using System.Net;
using System.Net.Sockets;

namespace Nameless.Helpers;

/// <summary>
///     Network related helper.
/// </summary>
public static class NetworkPortFinder {
    /// <summary>
    ///     Retrieves the first available port.
    /// </summary>
    /// <param name="start">
    ///     The start range.
    /// </param>
    /// <param name="limit">
    ///     The limit range.
    /// </param>
    /// <returns>
    ///     The first available port number.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     If not port is available.
    /// </exception>
    public static int GetFirstAvailablePort(int start = 10000, int limit = 100) {
        var port = ListAvailablePorts(start, limit).FirstOrDefault();

        return port == IPEndPoint.MinPort
            ? throw new InvalidOperationException("No available ports found in the specified range.")
            : port;
    }

    /// <summary>
    ///     Returns a list of available ports.
    /// </summary>
    /// <param name="start">
    ///     The start range.
    /// </param>
    /// <param name="limit">
    ///     The limit range.
    /// </param>
    /// <returns>
    ///     A list of available port numbers.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     if <paramref name="start"/> or <paramref name="limit"/> are
    ///     out of the range for IP endpoint port.
    /// </exception>
    public static IEnumerable<int> ListAvailablePorts(int start = 10000, int limit = 100) {
        var end = start + limit;

        if (start < IPEndPoint.MinPort || end > IPEndPoint.MaxPort || start > end) {
            throw new ArgumentOutOfRangeException(nameof(start), "Invalid port range.");
        }

        for (var port = start; port < end; port++) {
            if (IsPortFree(port)) {
                yield return port;
            }
        }
    }

    private static bool IsPortFree(int port) {
        return CheckSocket(port, SocketType.Stream, ProtocolType.Tcp) &&
               CheckSocket(port, SocketType.Dgram, ProtocolType.Udp);
    }

    private static bool CheckSocket(int port, SocketType socket, ProtocolType protocol) {
        try {
            using var instance = new Socket(
                AddressFamily.InterNetwork,
                socket,
                protocol
            );

            // Set socket options to allow address reuse
            instance.ExclusiveAddressUse = true;

            // Try to bind the socket to the specified port
            // it's enough to determine if the port is available
            instance.Bind(new IPEndPoint(IPAddress.Loopback, port));
        }
        catch { return false; }

        return true;
    }
}