using System.Linq.Expressions;
using Moq;
using Moq.Language;
using Moq.Language.Flow;
using Moq.Protected;

namespace Nameless.Testing.Tools.Mockers;

/// <summary>
///     <see cref="Mock{T}"/> wrapper.
/// </summary>
/// <typeparam name="TService">
///     The type of the service to mock.
/// </typeparam>
public class MockWrapper<TService> where TService : class {
    private readonly Mock<TService> _mock;
    private readonly MockSequence? _sequence;

    /// <summary>
    ///     Exposes the mocked object instance.
    /// </summary>
    public TService Object => _mock.Object;

    /// <summary>
    ///     Gets list of invocations which have been performed on this mock.
    /// </summary>
    public IInvocationList Invocations => _mock.Invocations;

    /// <summary>
    ///     Initializes a new instance of
    ///     <see cref="MockWrapper{TService}"/> class.
    /// </summary>
    /// <param name="mock">
    ///     The mock to wrap.
    /// </param>
    /// <param name="sequence">
    ///     The mock sequence instance to use.
    /// </param>
    public MockWrapper(Mock<TService> mock, MockSequence? sequence) {
        _mock = mock;
        _sequence = sequence;
    }

    /// <summary>
    ///     Specifies a setup on the mocked type for a call to a property
    ///     getter.
    /// </summary>
    /// <typeparam name="TProperty">
    ///     The type of the property.
    /// </typeparam>
    /// <param name="expression">
    ///     Lambda expression that specifies the property getter.
    /// </param>
    /// <returns>
    ///     The current <see cref="ISetupGetter{TService, TProperty}"/> so
    ///     other actions can be chained.
    /// </returns>
    public ISetupGetter<TService, TProperty> SetupGet<TProperty>(Expression<Func<TService, TProperty>> expression) {
        return _sequence is not null
            ? _mock.InSequence(_sequence).SetupGet(expression)
            : _mock.SetupGet(expression);
    }

    /// <summary>
    ///     Specifies a setup on the mocked type for a call to a property
    ///     setter.
    /// </summary>
    /// <typeparam name="TProperty">
    ///     The type of the property.
    /// </typeparam>
    /// <param name="expression">
    ///     The Lambda expression that sets a property to a value.
    /// </param>
    /// <returns>
    ///     The current <see cref="ISetupSetter{TService, TProperty}"/> so
    ///     other actions can be chained.
    /// </returns>
    public ISetupSetter<TService, TProperty> SetupSet<TProperty>(Action<TService> expression) {
        return _sequence is not null
            ? _mock.InSequence(_sequence).SetupSet<TProperty>(expression)
            : _mock.SetupSet<TProperty>(expression);
    }

    /// <summary>
    ///     Specifies a setup on the mocked type for a call to a property
    ///     setter.
    /// </summary>
    /// <param name="expression">
    ///     Lambda expression that sets a property to a value.
    /// </param>
    /// <returns>
    ///     The current <see cref="ISetup{TService}"/> so
    ///     other actions can be chained.
    /// </returns>
    public ISetup<TService> SetupSet(Action<TService> expression) {
        return _sequence is not null
            ? _mock.InSequence(_sequence).SetupSet(expression)
            : _mock.SetupSet(expression);
    }

    /// <summary>
    ///     Specifies a setup on the mocked type for a call to a
    ///     <see langword="void"/> method.
    /// </summary>
    /// <param name="expression">
    ///     Lambda expression that specifies the expected method invocation.
    /// </param>
    /// <returns>
    ///     The current <see cref="ISetup{TService}"/> so
    ///     other actions can be chained.
    /// </returns>
    public ISetup<TService> Setup(Expression<Action<TService>> expression) {
        return _sequence is not null
            ? _mock.InSequence(_sequence).Setup(expression)
            : _mock.Setup(expression);
    }

    /// <summary>
    ///     Specifies a setup on the mocked type for a call to a
    ///     non-<see langword="void"/> (value-returning) method.
    /// </summary>
    /// <typeparam name="TResult">
    ///     Type of the return value.
    /// </typeparam>
    /// <param name="expression">
    ///     Lambda expression that specifies the method invocation.
    /// </param>
    /// <returns>
    ///     The current <see cref="ISetup{TService, TResult}"/> so
    ///     other actions can be chained.
    /// </returns>
    public ISetup<TService, TResult> Setup<TResult>(Expression<Func<TService, TResult>> expression) {
        return _sequence is not null
            ? _mock.InSequence(_sequence).Setup(expression)
            : _mock.Setup(expression);
    }

    /// <summary>
    ///     Performs a sequence of actions, one per call.
    /// </summary>
    /// <param name="expression">
    ///     The sequence lambda expression.
    /// </param>
    /// <returns>
    ///     The current <see cref="ISetupSequentialAction"/> so
    ///     other actions can be chained.
    /// </returns>
    public ISetupSequentialAction SetupSequence(Expression<Action<TService>> expression) {
        return _mock.SetupSequence(expression);
    }

    /// <summary>
    ///     Return a sequence of values, once per call.
    /// </summary>
    /// <typeparam name="TResult">
    ///     The type of the result.
    /// </typeparam>
    /// <param name="expression">
    ///     The sequence lambda expression.
    /// </param>
    /// <returns>
    ///     The current <see cref="ISetupSequentialResult{TResult}"/> so
    ///     other actions can be chained.
    /// </returns>
    public ISetupSequentialResult<TResult> SetupSequence<TResult>(Expression<Func<TService, TResult>> expression) {
        return _mock.SetupSequence(expression);
    }

    /// <summary>
    ///     Enable protected setups for the mock.
    /// </summary>
    /// <returns>
    ///     The current <see cref="IProtectedMock{TService}"/> so
    ///     other actions can be chained.
    /// </returns>
    public IProtectedMock<TService> Protected() {
        return _mock.Protected();
    }

    /// <summary>
    ///     Adds an interface implementation to the mock, allowing setups to be specified for it.
    /// </summary>
    /// <typeparam name="TOtherService">
    ///     The type of other service.
    /// </typeparam>
    /// <returns>
    ///     The current <see cref="MockWrapper{TOtherService}"/> so
    ///     other actions can be chained.
    /// </returns>
    public MockWrapper<TOtherService> As<TOtherService>() where TOtherService : class {
        return new MockWrapper<TOtherService>(
            _mock.As<TOtherService>(),
            _sequence
        );
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
    /// <param name="failMessage">
    ///     Message to show if verification fails.
    /// </param>
    public void VerifyGet<TProperty>(Expression<Func<TService, TProperty>> expression, Times times, string? failMessage = null) {
        _mock.VerifyGet(expression, times, failMessage ?? string.Empty);
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
    /// <param name="failMessage">
    ///     Message to show if verification fails.
    /// </param>
    public void VerifySet(Action<TService> action, Times times, string? failMessage = null) {
        _mock.VerifySet(action, times, failMessage ?? string.Empty);
    }

    /// <summary>
    ///     Verifies that a specific invocation matching the given expression
    ///     was performed on the mock, specifying a failure error message.
    ///     Use in conjunction with the default
    ///     <see cref="MockBehavior.Loose"/>.
    /// </summary>
    /// <param name="expression">
    ///     Expression to verify.
    /// </param>
    /// <param name="times">
    ///     The number of times a method is expected to be called.
    /// </param>
    /// <param name="failMessage">
    ///     Message to show if verification fails.
    /// </param>
    public void Verify(Expression<Action<TService>> expression, Times times, string? failMessage = null) {
        _mock.Verify(expression, times, failMessage ?? string.Empty);
    }

    /// <summary>
    ///     Verifies that a specific invocation matching the given expression
    ///     was performed on the mock, specifying a failure error message.
    /// </summary>
    /// <typeparam name="TResult">
    ///     The type of the result.
    /// </typeparam>
    /// <param name="expression">
    ///     Expression to verify.
    /// </param>
    /// <param name="times">
    ///     The number of times a method is expected to be called.
    /// </param>
    /// <param name="failMessage">
    ///     Message to show if verification fails.
    /// </param>
    public void Verify<TResult>(Expression<Func<TService, TResult>> expression, Times times, string? failMessage = null) {
        _mock.Verify(expression, times, failMessage ?? string.Empty);
    }
}