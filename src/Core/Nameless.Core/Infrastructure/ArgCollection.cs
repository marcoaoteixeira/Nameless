using System.Collections;

namespace Nameless.Infrastructure;

public class ArgCollection : IEnumerable<Arg> {
    private readonly Dictionary<string, object> _dictionary = [];

    public void Set(Arg arg) {
        Prevent.Argument.Null(arg, nameof(arg));

        Set(arg.Name, arg.Value);
    }

    public void Set(string name, object value) {
        Prevent.Argument.NullOrWhiteSpace(name, nameof(name));
        Prevent.Argument.Null(value, nameof(value));

        _dictionary[name] = value;
    }

    public object? Get(string name)
        => _dictionary.GetValueOrDefault(name);

    public IEnumerator<Arg> GetEnumerator()
        => GetArgs()
            .GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => GetArgs()
            .GetEnumerator();

    private IEnumerable<Arg> GetArgs()
        => _dictionary.Select(item => new Arg(item.Key, item.Value));
}