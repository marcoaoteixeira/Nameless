using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using Nameless.Mediator.Streams;

namespace Nameless.Mediator;

internal record StrStream : IStream<string>;

internal sealed class StrStreamHandler : IStreamHandler<StrStream, string> {
    public async IAsyncEnumerable<string> HandleAsync(StrStream request, [EnumeratorCancellation] CancellationToken cancellationToken) {
        await Task.Yield();

        yield return "a";
        yield return "b";
    }
}

internal record DualStream : IStream<string>, IStream<int>;

internal sealed class DualStreamHandler : IStreamHandler<DualStream, string>, IStreamHandler<DualStream, int> {
    public async IAsyncEnumerable<string> HandleAsync(DualStream request, [EnumeratorCancellation] CancellationToken cancellationToken) {
        await Task.Yield();

        yield return "text";
    }

    async IAsyncEnumerable<int> IStreamHandler<DualStream, int>.HandleAsync(DualStream request, [EnumeratorCancellation] CancellationToken cancellationToken) {
        await Task.Yield();

        yield return 42;
    }
}

[UnitTest]
public class StreamHandlerInvokerTests {
    private static ServiceProvider BuildProvider() {
        var services = new ServiceCollection();
        services.RegisterMediator(r => r.WithUseAssemblyScan(false)
                                        .WithStreamHandler(typeof(StrStreamHandler))
                                        .WithStreamHandler(typeof(DualStreamHandler)));

        return services.BuildServiceProvider();
    }

    private static async Task<List<T>> ToListAsync<T>(IAsyncEnumerable<T> items) {
        var result = new List<T>();
        await foreach (var item in items) {
            result.Add(item);
        }

        return result;
    }

    [Fact]
    public async Task CreateAsync_StreamViewedAsCovariantResponse_ThrowsInvalidOperationException() {
        // arrange
        using var provider = BuildProvider();
        var sut = new StreamHandlerInvoker(provider);
        IStream<object> covariant = new StrStream();

        // act
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => ToListAsync(sut.CreateAsync(covariant, TestContext.Current.CancellationToken)));

        // assert
        Assert.Contains(nameof(StrStream), exception.Message);
    }

    [Fact]
    public async Task CreateAsync_CovariantCallThenTypedCall_TypedCallStillWorks() {
        // arrange
        using var provider = BuildProvider();
        var sut = new StreamHandlerInvoker(provider);
        IStream<object> covariant = new StrStream();
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => ToListAsync(sut.CreateAsync(covariant, TestContext.Current.CancellationToken)));

        // act
        var items = await ToListAsync(sut.CreateAsync(new StrStream(), TestContext.Current.CancellationToken));

        // assert
        Assert.Equal(["a", "b"], items);
    }

    [Fact]
    public async Task CreateAsync_StreamDeclaringSeveralResponses_DispatchesEachOne() {
        // arrange
        using var provider = BuildProvider();
        var sut = new StreamHandlerInvoker(provider);

        // act
        var text = await ToListAsync(sut.CreateAsync<string>(new DualStream(), TestContext.Current.CancellationToken));
        var numbers = await ToListAsync(sut.CreateAsync<int>(new DualStream(), TestContext.Current.CancellationToken));

        // assert
        Assert.Multiple(
            () => Assert.Equal(["text"], text),
            () => Assert.Equal([42], numbers)
        );
    }

    [Fact]
    public void CreateAsync_WithNullRequest_ThrowsArgumentNullException() {
        // arrange
        using var provider = BuildProvider();
        var sut = new StreamHandlerInvoker(provider);

        // act & assert
        Assert.Throws<ArgumentNullException>(
            () => sut.CreateAsync<string>(null!, TestContext.Current.CancellationToken));
    }
}
