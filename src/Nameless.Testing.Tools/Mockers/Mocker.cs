using System.Linq.Expressions;
using Moq;

namespace Nameless.Testing.Tools.Mockers;

public abstract class Mocker<TService> where TService : class {
    protected MockWrapper<TService> MockInstance { get; }

    protected Mocker()
        : this(MockBehavior.Loose, sequence: null, args: []) { }

    protected Mocker(MockBehavior behavior)
        : this(behavior, sequence: null, args: []) { }

    protected Mocker(MockSequence? sequence)
        : this(MockBehavior.Strict, sequence, args: []) { }

    protected Mocker(object[] args)
        : this(MockBehavior.Loose, sequence: null, args) { }

    protected Mocker(MockBehavior behavior, object[] args)
        : this(behavior, sequence: null, args) { }

    protected Mocker(MockSequence? sequence, object[] args)
        : this(MockBehavior.Strict, sequence, args) { }

    protected Mocker(MockBehavior behavior, MockSequence? sequence, object[] args) {
        behavior = sequence is not null
            ? MockBehavior.Strict
            : behavior;

        MockInstance = new MockWrapper<TService>(
            mock: new Mock<TService>(behavior, args),
            sequence
        );
    }

    public virtual TService Build() {
        return MockInstance.Object;
    }

    public void VerifyGet<TProperty>(Expression<Func<TService, TProperty>> expression, int times = 1, bool exactly = false, string? failMessage = null) {
        MockInstance.VerifyGet(expression, GetTimes(times, exactly), failMessage);
    }

    public void VerifySet(Action<TService> action, int times = 1, bool exactly = false, string? failMessage = null) {
        MockInstance.VerifySet(action, GetTimes(times, exactly), failMessage);
    }
    
    public void Verify(Expression<Action<TService>> expression, int times = 1, bool exactly = false, string? failMessage = null) {
        MockInstance.Verify(expression, GetTimes(times, exactly), failMessage);
    }

    public void Verify(Expression<Func<TService, bool>> expression, int times = 1, bool exactly = false, string? failMessage = null) {
        MockInstance.Verify(expression, GetTimes(times, exactly), failMessage);
    }

    public void ResetVerificationCount() {
        MockInstance.Invocations.Clear();
    }

    private static Times GetTimes(int times, bool exactly) {
        return times switch {
            0 => Times.Never(),
            1 => exactly ? Times.Exactly(1) : Times.Once(),
            _ => exactly ? Times.Exactly(times) : Times.AtLeast(times)
        };
    }
}