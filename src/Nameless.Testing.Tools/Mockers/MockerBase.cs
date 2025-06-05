using System.Linq.Expressions;
using Moq;

namespace Nameless.Testing.Tools.Mockers;

public abstract class MockerBase<TService> where TService : class {
    protected Mock<TService> MockInstance { get; } = new();

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