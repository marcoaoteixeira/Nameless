using System.Net;
using System.Net.Sockets;

namespace Nameless.Helpers;

public static class NetworkPortFinder {
    public static int GetFirstAvailablePort(int start = 10000, int limit = 100) {
        var port = ListAvailablePorts(start, limit).FirstOrDefault();

        return port == IPEndPoint.MinPort
            ? throw new InvalidOperationException("No available ports found in the specified range.")
            : port;
    }

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