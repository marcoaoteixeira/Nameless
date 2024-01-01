using Microsoft.Extensions.Primitives;

namespace Nameless.Localization.Microsoft.Json {
    public class FakeChangeToken : IChangeToken {
        private readonly Dictionary<Action<object?>, object?> _callbacks = [];
        private readonly IDisposable _disposable;

        private bool _hasChanged;
        public bool HasChanged => _hasChanged;

        private bool _activeChangeCallbacks;
        public bool ActiveChangeCallbacks => _activeChangeCallbacks;

        public FakeChangeToken(IDisposable disposable) {
            _disposable = disposable;
        }

        public IDisposable RegisterChangeCallback(Action<object?> callback, object? state) {
            _callbacks.Add(callback, state);
            return _disposable;
        }

        public void Trigger() {
            foreach (var kvp in _callbacks) {
                kvp.Key?.Invoke(kvp.Value);
            }

            _hasChanged = true;
            _activeChangeCallbacks = true;
        }
    }
}
