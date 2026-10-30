using System.Linq.Expressions;
using Moq;

namespace Nameless.Testing.Tools.Mockers;

/// <summary>
///     Mocker base class.
/// </summary>
/// <typeparam name="TService">
///     Type of service to mock.
/// </typeparam>
public abstract class Mocker<TService> where TService : class {
    /// <summary>
    ///     Gets the mock instance.
    /// </summary>
    protected MockWrapper<TService> MockInstance { get; }

    /// <summary>
    ///     Mocker initializer.
    /// </summary>
    /// <param name="behavior">
    ///     The mocking behavior.
    /// </param>
    /// <param name="sequence">
    ///     The mock sequence context.
    /// </param>
    /// <param name="args">
    ///     Arguments when mocking an abstract class with constructor
    ///     parameters.
    /// </param>
    protected Mocker(MockBehavior behavior = MockBehavior.Loose, MockSequence? sequence = null, object[]? args = null) {
        behavior = sequence is not null
            ? MockBehavior.Strict
            : behavior;

        MockInstance = new MockWrapper<TService>(
            mock: new Mock<TService>(behavior, args ?? []),
            sequence
        );
    }

    /// <summary>
    ///     Builds the service.
    /// </summary>
    /// <returns>
    ///     A mocked instance of service <typeparamref name="TService"/>.
    /// </returns>
    public virtual TService Build() {
        return MockInstance.Object;
    }

    /// <summary>
    ///     Verifies that a property was read on the mock, specifying a failure error message.
    /// </summary>
    /// <typeparam name="TProperty">
    ///     The type of the property.
    /// </typeparam>
    /// <param name="expression">
    ///     Expression to verify.
    /// </param>
    /// <param name="times">
    ///     The number of times a method is expected to be called.
    /// </param>
    /// <param name="exactly">
    ///     Whether it should verify exactly number of calls or at least the
    ///     specified number.
    /// </param>
    /// <param name="failMessage">
    ///     Message to show if verification fails.
    /// </param>
    public void VerifyGet<TProperty>(Expression<Func<TService, TProperty>> expression, int times = 1, bool exactly = false, string? failMessage = null) {
        MockInstance.VerifyGet(expression, GetTimes(times, exactly), failMessage);
    }

    /// <summary>
    ///     Verifies that a property was set on the mock.
    /// </summary>
    /// <param name="action">
    ///     Expression to verify.
    /// </param>
    /// <param name="times">
    ///     The number of times a method is expected to be called.
    /// </param>
    /// <param name="exactly">
    ///     Whether it should verify exactly number of calls or at least the
    ///     specified number.
    /// </param>
    /// <param name="failMessage">
    ///     Message to show if verification fails.
    /// </param>
    public void VerifySet(Action<TService> action, int times = 1, bool exactly = false, string? failMessage = null) {
        MockInstance.VerifySet(action, GetTimes(times, exactly), failMessage);
    }

    /// <summary>
    ///     Verifies that a specific invocation matching the given expression
    ///     was performed on the mock, specifying a failure error message.
    /// </summary>
    /// <param name="expression">
    ///     Expression to verify.
    /// </param>
    /// <param name="times">
    ///     The number of times a method is expected to be called.
    /// </param>
    /// <param name="exactly">
    ///     Whether it should verify exactly number of calls or at least the
    ///     specified number.
    /// </param>
    /// <param name="failMessage">
    ///     Message to show if verification fails.
    /// </param>
    public void Verify(Expression<Action<TService>> expression, int times = 1, bool exactly = false, string? failMessage = null) {
        MockInstance.Verify(expression, GetTimes(times, exactly), failMessage);
    }

    /// <summary>
    ///     Verifies that a specific invocation matching the given expression
    ///     was performed on the mock, specifying a failure error message.
    /// </summary>
    /// <param name="expression">
    ///     Expression to verify.
    /// </param>
    /// <param name="times">
    ///     The number of times a method is expected to be called.
    /// </param>
    /// <param name="exactly">
    ///     Whether it should verify exactly number of calls or at least the
    ///     specified number.
    /// </param>
    /// <param name="failMessage">
    ///     Message to show if verification fails.
    /// </param>
    public void Verify(Expression<Func<TService, bool>> expression, int times = 1, bool exactly = false, string? failMessage = null) {
        MockInstance.Verify(expression, GetTimes(times, exactly), failMessage);
    }

    /// <summary>
    ///     Clears the list of invocations.
    /// </summary>
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