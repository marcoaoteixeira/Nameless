using System.Linq.Expressions;
using Moq;
using Moq.Language;
using Moq.Language.Flow;
using Moq.Protected;

namespace Nameless.Testing.Tools.Mockers;

public class MockWrapper<TService> where TService : class {
    private readonly Mock<TService> _mock;
    private readonly MockSequence? _sequence;

    public TService Object => _mock.Object;

    public IInvocationList Invocations => _mock.Invocations;

    public MockWrapper(Mock<TService> mock, MockSequence? sequence) {
        _mock = mock;
        _sequence = sequence;
    }

    public ISetupGetter<TService, TResult> SetupGet<TResult>(Expression<Func<TService, TResult>> expression) {
        return _sequence is not null
            ? _mock.InSequence(_sequence).SetupGet(expression)
            : _mock.SetupGet(expression);
    }

    public ISetupSetter<TService, TProperty> SetupSet<TProperty>(Action<TService> expression) {
        return _sequence is not null
            ? _mock.InSequence(_sequence).SetupSet<TProperty>(expression)
            : _mock.SetupSet<TProperty>(expression);
    }

    public ISetup<TService> SetupSet(Action<TService> expression) {
        return _sequence is not null
            ? _mock.InSequence(_sequence).SetupSet(expression)
            : _mock.SetupSet(expression);
    }

    public ISetup<TService> Setup(Expression<Action<TService>> expression) {
        return _sequence is not null
            ? _mock.InSequence(_sequence).Setup(expression)
            : _mock.Setup(expression);
    }

    public ISetup<TService, TResult> Setup<TResult>(Expression<Func<TService, TResult>> expression) {
        return _sequence is not null
            ? _mock.InSequence(_sequence).Setup(expression)
            : _mock.Setup(expression);
    }

    public ISetupSequentialAction SetupSequence(Expression<Action<TService>> expression) {
        return _mock.SetupSequence(expression);
    }

    public ISetupSequentialResult<TResult> SetupSequence<TResult>(Expression<Func<TService, TResult>> expression) {
        return _mock.SetupSequence(expression);
    }

    public IProtectedMock<TService> Protected() {
        return _mock.Protected();
    }

    public MockWrapper<TOtherService> As<TOtherService>() where TOtherService : class {
        return new MockWrapper<TOtherService>(
            _mock.As<TOtherService>(),
            _sequence
        );
    }

    public void VerifyGet<TProperty>(Expression<Func<TService, TProperty>> expression, Times times, string? failMessage = null) {
        _mock.VerifyGet(expression, times, failMessage ?? string.Empty);
    }

    public void VerifySet(Action<TService> expression, Times times, string? failMessage = null) {
        _mock.VerifySet(expression, times, failMessage ?? string.Empty);
    }

    public void Verify(Expression<Action<TService>> expression, Times times, string? failMessage = null) {
        _mock.Verify(expression, times, failMessage ?? string.Empty);
    }

    public void Verify<TResult>(Expression<Func<TService, TResult>> expression, Times times, string? failMessage = null) {
        _mock.Verify(expression, times, failMessage ?? string.Empty);
    }
}