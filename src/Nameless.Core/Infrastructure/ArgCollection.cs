using System.Collections;

namespace Nameless.Infrastructure;

public class ArgCollection : ICollection<Arg> {
    private Dictionary<string, object?> Args { get; } = new(StringComparer.Ordinal);

    public object? this[string name] {
        get => Get(name);
        set => Set(name, value);
    }

    public int Count => Args.Count;

    public bool IsReadOnly => false;

    public ArgCollection() {
        
    }

    public ArgCollection(IEnumerable<Arg> args) {
        foreach (var arg in Prevent.Argument.Null(args)) {
            Add(arg);
        }
    }

    public void Add(Arg item) {
        Prevent.Argument.Null(item);

        Add(item.Name, item.Value);
    }

    public void Add(string name, object? value) {
        Prevent.Argument.NullOrWhiteSpace(name);

        Args[name] = value;
    }

    public void Clear() => Args.Clear();

    public bool Contains(Arg item) {
        Prevent.Argument.Null(item);

        return Contains(item.Name);
    }

    public bool Contains(string name) {
        Prevent.Argument.Null(name);

        return Args.ContainsKey(name);
    }

    public void CopyTo(Arg[] array, int arrayIndex) {
        Prevent.Argument.Null(array);
        Prevent.Argument.LowerThan(arrayIndex, minValue: 0);

        var sourceArray = GetArgs().ToArray();

        Array.Copy(sourceArray: sourceArray,
                   sourceIndex: 0,
                   destinationArray: array,
                   destinationIndex: arrayIndex,
                   length: sourceArray.Length);
    }

    public bool Remove(Arg item) {
        Prevent.Argument.Null(item);

        return Remove(item.Name);
    }

    public bool Remove(string name) {
        Prevent.Argument.Null(name);

        return Args.Remove(name);
    }

    public IEnumerator<Arg> GetEnumerator()
        => GetArgs().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => GetArgs().GetEnumerator();

    private IEnumerable<Arg> GetArgs()
        => Args.Select(item => new Arg(item.Key, item.Value));

    private object? Get(string argumentName) {
        Prevent.Argument.NullOrWhiteSpace(argumentName);

        return Args.GetValueOrDefault(argumentName);
    }

    private void Set(string argumentName, object? argumentValue)
        => Add(argumentName, argumentValue);
}