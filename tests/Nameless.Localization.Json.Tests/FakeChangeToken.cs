﻿using Microsoft.Extensions.Primitives;

namespace Nameless.Localization.Json;

public class FakeChangeToken : IChangeToken {
    private readonly Dictionary<Action<object>, object> _callbacks = [];
    private readonly IDisposable _disposable;

    public FakeChangeToken(IDisposable disposable) {
        _disposable = disposable;
    }

    public bool HasChanged { get; private set; }
    public bool ActiveChangeCallbacks { get; private set; }

    public IDisposable RegisterChangeCallback(Action<object> callback, object state) {
        _callbacks.Add(callback, state);
        return _disposable;
    }

    public void Trigger() {
        foreach (var kvp in _callbacks) {
            kvp.Key?.Invoke(kvp.Value);
        }

        HasChanged = true;
        ActiveChangeCallbacks = true;
    }
}