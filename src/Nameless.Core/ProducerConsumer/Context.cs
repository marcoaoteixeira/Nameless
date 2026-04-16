using System.Collections;
using System.Collections.Concurrent;

namespace Nameless.ProducerConsumer;

public abstract class Context : IEnumerable<KeyValuePair<string, object?>> {
    private readonly ConcurrentDictionary<string, object?> _props = new(StringComparer.OrdinalIgnoreCase);

    public object? this[string key] {
        get => _props.GetValueOrDefault(Throws.When.NullOrWhiteSpace(key));
        set => _props[Throws.When.NullOrWhiteSpace(key)] = value;
    }

    protected Context(IDictionary<string, object?> dictionary) {
        foreach (var kvp in dictionary) {
            this[kvp.Key] = kvp.Value;
        }
    }

    public void Add(KeyValuePair<string, object?> value) {
        this[value.Key] = value.Value;
    }

    public IEnumerator<KeyValuePair<string, object?>> GetEnumerator() {
        return _props.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }
}