using System.Linq.Expressions;
using Moq;

namespace Nameless.Mockers;

public abstract class MockerBase<T> : IVerifiable<T> where T : class {
    protected Mock<T> Mock { get; } = new();

    public virtual T Build()
        => Mock.Object;

    public void Verify(Expression<Action<T>> expression)
        => Verify(expression, Times.Once());

    public void Verify(Expression<Action<T>> expression, Times times)
        => Mock.Verify(expression, times);

    public void Verify(Expression<Func<T, bool>> expression)
        => Verify(expression, Times.Once());

    public void Verify(Expression<Func<T, bool>> expression, Times times)
        => Mock.Verify(expression, times);

    public void ResetVerificationCount()
        => Mock.Invocations.Clear();
}

public interface IVerifiable<T> where T : class {
    void Verify(Expression<Action<T>> expression);

    void Verify(Expression<Action<T>> expression, Times times);

    void Verify(Expression<Func<T, bool>> expression);

    void Verify(Expression<Func<T, bool>> expression, Times times);

    void ResetVerificationCount();
}