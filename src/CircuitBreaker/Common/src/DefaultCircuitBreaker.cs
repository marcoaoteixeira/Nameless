using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Nameless.Logging;

namespace Nameless.CircuitBreaker {
    public sealed class DefaultCircuitBreaker : ICircuitBreaker, IDisposable {
        #region Private Read-Only Fields

        private readonly CircuitBreakerSettings _settings;

        #endregion

        #region Private Fields

        private Timer _timer;
        private long _lastDueTime;
        private long _timeout;
        private long _threshold;
        private double _failureCounter;
        private IList<IExceptionFilter> _exceptionFilters;
        private bool _disposed;

        #endregion

        #region Public Properties

        private ILogger _logger;
        public ILogger Logger {
            get { return _logger ?? (_logger = NullLogger.Instance); }
            set { _logger = value ?? NullLogger.Instance; }
        }

        #endregion

        #region Public Constructors

        public DefaultCircuitBreaker (CircuitBreakerSettings settings) {
            Prevent.ParameterNull (settings, nameof (settings));

            _settings = settings;

            Initialize ();
        }

        #endregion

        #region Destructor

        ~DefaultCircuitBreaker () {
            Dispose (disposing: false);
        }

        #endregion

        #region Private Methods

        private void Initialize () {
            _threshold = _settings.Threshold;
            _timeout = _settings.Timeout;
            State = CircuitBreakerState.Closed;

            _lastDueTime = System.Threading.Timeout.Infinite;
            _timer = new Timer (TimerCallback, state : null, dueTime : _lastDueTime, period : Timeout);
            _exceptionFilters = new List<IExceptionFilter> ();
        }

        private void TimerCallback (object state) {
            if (State != CircuitBreakerState.Open) { return; }

            State = CircuitBreakerState.HalfOpen;
            OnStateChanged (new StateChangeEventArgs (State));
            StopTimer ();
        }

        private void StartTimer () {
            _lastDueTime = 0L; // start immediately
            _timer.Change (dueTime: _lastDueTime, period: Timeout);
        }

        private void StopTimer () {
            _lastDueTime = System.Threading.Timeout.Infinite; // stop immediately
            _timer.Change (dueTime: System.Threading.Timeout.Infinite, period: Timeout);
        }

        private void ResetTimer () {
            _timer.Change (dueTime: _lastDueTime, period: Timeout);
        }

        private void OnStateChanged (StateChangeEventArgs e) => StateChanged?.Invoke (this, e);

        private void OnServiceLevelChanged (ServiceLevelEventArgs e) => ServiceLevelChanged?.Invoke (this, e);

        private void BlockAccessAfterDispose () {
            if (_disposed) {
                throw new ObjectDisposedException (GetType ().FullName);
            }
        }

        private void Dispose (bool disposing) {
            if (_disposed) { return; }
            if (disposing) {
                if (_timer != null) {
                    _timer.Dispose ();
                }
            }

            _timer = null;
            _disposed = true;
        }

        #endregion

        #region ICircuitBreaker Members

        public event EventHandler<StateChangeEventArgs> StateChanged;

        public event EventHandler<ServiceLevelEventArgs> ServiceLevelChanged;

        public long Threshold { get; set; }

        public long Timeout {
            get { return _timeout; }
            set {
                if (value < 0) {
                    throw new ArgumentOutOfRangeException (nameof (value), value, "Parameter must be greater than zero.");
                }

                _timeout = value;
                ResetTimer ();
            }
        }

        public double ServiceLevel => (Threshold - _failureCounter) / Threshold * 100D;

        public CircuitBreakerState State { get; private set; }

        public TResult Execute<TResult> (Delegate operation, params object[] arguments) {
            Prevent.ParameterNull (operation, nameof (operation));

            // Ensure no null arguments.
            arguments = arguments ?? Array.Empty<object> ();

            if (State == CircuitBreakerState.Open) {
                throw new CircuitOpenedException ();
            }

            TResult result = default;
            try {
                result = (TResult) operation.DynamicInvoke (arguments);

                if (result is Task task) {

                    task.GetAwaiter ().GetResult ();
                }
            } catch (Exception ex) {
                // If there is no inner exception, then the exception was caused by the invoker, so throw.
                if (ex.InnerException == null) { throw; }

                // If exception is one of the ignored types, then throw original exception
                if (_exceptionFilters.Any (filter => filter.Ignore (ex.InnerException))) {
                    throw ex.InnerException;
                }

                // Operation failed in a half-open state, so reopen circuit
                if (State == CircuitBreakerState.HalfOpen) { Trip (); }
                // Operation failed in an open state, so increment failure count and throw exception
                else if (_failureCounter < Threshold) {
                    ++_failureCounter;
                    OnServiceLevelChanged (new ServiceLevelEventArgs (ServiceLevel));
                }
                // Failure count has reached threshold, so trip circuit breaker
                else if (_failureCounter >= Threshold) { Trip (); }

                throw new OperationFailedException ($"{operation.Target.GetType ().FullName}::{operation.Method.Name}", ex.InnerException);
            }

            // If operation succeeded without error and circuit breaker
            // is in a half-open state, then reset
            if (State == CircuitBreakerState.HalfOpen) { Reset (); }

            if (_failureCounter > 0) {
                --_failureCounter;
                OnServiceLevelChanged (new ServiceLevelEventArgs (ServiceLevel));
            }

            return result;
        }

        public void IgnoreOn (IExceptionFilter filter) {
            throw new NotImplementedException ();
        }

        public void Reset () {
            BlockAccessAfterDispose ();

            if (State == CircuitBreakerState.Closed) { return; }

            State = CircuitBreakerState.Closed;
            OnStateChanged (new StateChangeEventArgs (State));
            StopTimer ();
        }

        public void Trip () {
            BlockAccessAfterDispose ();

            if (State == CircuitBreakerState.Open) { return; }

            State = CircuitBreakerState.Open;
            OnStateChanged (new StateChangeEventArgs (State));
            StartTimer ();
        }

        #endregion

        #region IDisposable Members

        public void Dispose () {
            Dispose (disposing: true);
            GC.SuppressFinalize (this);
        }

        #endregion
    }
}