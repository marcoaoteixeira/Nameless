using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace Nameless.Mediator.Streams.Fixtures;

public class SimpleStreamHandler : IStreamHandler<SimpleStream, string> {
    private readonly ILogger _logger;

    public SimpleStreamHandler(ILogger logger) {
        _logger = logger;
    }

    public async IAsyncEnumerable<string> HandleAsync(SimpleStream request, [EnumeratorCancellation] CancellationToken cancellationToken) {
        foreach (var message in request.Messages) {
            _logger.LogDebug("{StreamHandler}: {Message}", GetType().Name, message);

            yield return message;
        }

        await Task.Yield();
    }
}