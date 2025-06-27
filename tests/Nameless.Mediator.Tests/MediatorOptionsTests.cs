using System.Reflection;
using System.Runtime.CompilerServices;
using Nameless.Mediator.Requests;
using Nameless.Mediator.Streams;

namespace Nameless.Mediator;

public class MediatorOptionsTests {
    [Fact]
    public void WhenInitializing_WithValidValues_ThenRetrieveCorrectValuesFromProperties() {
        // arrange
        Assembly[] assemblies = [typeof(MediatorOptionsTests).Assembly];
        const bool UseEventHandlers = false;
        const bool UseRequestHandlers = false;
        const bool UseStreamHandlers = false;

        // act
        var sut = new MediatorOptions {
            Assemblies = assemblies,
            UseEventHandlers = UseEventHandlers,
            UseRequestHandlers = UseRequestHandlers,
            UseStreamHandlers = UseStreamHandlers
        };

        // assert
        Assert.Multiple(() => {
            Assert.Equivalent(assemblies, sut.Assemblies);
            Assert.Equal(UseEventHandlers, sut.UseEventHandlers);
            Assert.Equal(UseRequestHandlers, sut.UseRequestHandlers);
            Assert.Equal(UseStreamHandlers, sut.UseStreamHandlers);
        });
    }

    [Fact]
    public void WhenRegisteringRequestPipelineBehavior_ThenPropertyShouldContainsType() {
        // arrange

        // act
        var sut = new MediatorOptions()
           .RegisterRequestPipelineBehavior(typeof(FakeRequestPipelineBehavior));

        // assert
        Assert.Contains(sut.RequestPipelineBehaviors, item => typeof(FakeRequestPipelineBehavior).IsAssignableFrom(item));
    }

    [Fact]
    public void WhenRegisteringSameRequestPipelineBehaviorMultipleTimes_ThenRequestPipelineBehaviorsShouldContainsOnlyOneCopy() {
        // arrange

        // act
        var sut = new MediatorOptions().RegisterRequestPipelineBehavior(typeof(FakeRequestPipelineBehavior))
                                       .RegisterRequestPipelineBehavior(typeof(FakeRequestPipelineBehavior))
                                       .RegisterRequestPipelineBehavior(typeof(FakeRequestPipelineBehavior));

        // assert
        Assert.Multiple(() => {
            Assert.Single(sut.RequestPipelineBehaviors);
            Assert.Contains(sut.RequestPipelineBehaviors, item => typeof(FakeRequestPipelineBehavior).IsAssignableFrom(item));
        });
    }

    [Fact]
    public void WhenRegisteringRequestPipelineBehavior_WhenTypeIsNotAssignable_ThenThrowsException() {
        // arrange

        // act
        var exception = Record.Exception(() => new MediatorOptions().RegisterRequestPipelineBehavior(typeof(string)));

        // assert
        Assert.IsType<InvalidOperationException>(exception);
    }

    [Fact]
    public void WhenRegisteringStreamPipelineBehavior_ThenPropertyShouldContainsType() {
        // arrange

        // act
        var sut = new MediatorOptions()
           .RegisterStreamPipelineBehavior(typeof(FakeStreamPipelineBehavior));

        // assert
        Assert.Contains(sut.StreamPipelineBehaviors, item => typeof(FakeStreamPipelineBehavior).IsAssignableFrom(item));
    }

    [Fact]
    public void WhenRegisteringSameStreamPipelineBehaviorMultipleTimes_ThenRequestPipelineBehaviorsShouldContainsOnlyOneCopy() {
        // arrange

        // act
        var sut = new MediatorOptions().RegisterStreamPipelineBehavior(typeof(FakeStreamPipelineBehavior))
                                       .RegisterStreamPipelineBehavior(typeof(FakeStreamPipelineBehavior))
                                       .RegisterStreamPipelineBehavior(typeof(FakeStreamPipelineBehavior));

        // assert
        Assert.Multiple(() => {
            Assert.Single(sut.StreamPipelineBehaviors);
            Assert.Contains(sut.StreamPipelineBehaviors, item => typeof(FakeStreamPipelineBehavior).IsAssignableFrom(item));
        });
    }

    [Fact]
    public void WhenRegisteringStreamPipelineBehavior_WhenTypeIsNotAssignable_ThenThrowsException() {
        // arrange

        // act
        var exception = Record.Exception(() => new MediatorOptions().RegisterStreamPipelineBehavior(typeof(string)));

        // assert
        Assert.IsType<InvalidOperationException>(exception);
    }

    public class FakeRequestPipelineBehavior : IRequestPipelineBehavior<object, object> {
        public Task<object> HandleAsync(object request, RequestHandlerDelegate<object> next, CancellationToken cancellationToken) {
            return Task.FromResult((object)123);
        }
    }

    public class FakeStreamPipelineBehavior : IStreamPipelineBehavior<object, object> {
        public async IAsyncEnumerable<object> HandleAsync(object request, StreamHandlerDelegate<object> next, [EnumeratorCancellation] CancellationToken cancellationToken) {
            yield return 123;

            await Task.Yield();
        }
    }
}
