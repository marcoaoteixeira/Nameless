using System.Linq.Expressions;
using Moq;

namespace Nameless.Testing.Tools.Mockers;

public abstract class Mocker<TService> where TService : class {
    private readonly Mock<TService> _mockInstance;
    private readonly MockSequence? _sequence;

    protected Mock<TService> MockInstance {
        get {
            if (_sequence is not null) {
                _mockInstance.InSequence(_sequence);
            }

            return _mockInstance;
        }
    }

    protected Mocker()
        : this(MockBehavior.Default, useSequence: false) { }

    protected Mocker(bool useSequence)
        : this(MockBehavior.Default, useSequence) { }

    protected Mocker(MockBehavior behavior, bool useSequence) {
        _mockInstance = new Mock<TService>(behavior);
        _sequence = useSequence ? new MockSequence() : null;
    }

    public virtual TService Build() {
        return MockInstance.Object;
    }

    public void Verify(Expression<Action<TService>> expression) {
        Verify(expression, Times.Once());
    }

    public void Verify(Expression<Action<TService>> expression, Times times) {
        MockInstance.Verify(expression, times);
    }

    public void Verify(Expression<Func<TService, bool>> expression) {
        Verify(expression, Times.Once());
    }

    public void Verify(Expression<Func<TService, bool>> expression, Times times) {
        MockInstance.Verify(expression, times);
    }

    public void ResetVerificationCount() {
        MockInstance.Invocations.Clear();
    }
}