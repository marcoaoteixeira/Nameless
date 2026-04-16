using System.Collections;

namespace Nameless.Data;

public class ParameterCollection : IEnumerable<Parameter> {
    private readonly Dictionary<string, Parameter> _inner = new(StringComparer.OrdinalIgnoreCase);

    public ParameterCollection() { }

    public ParameterCollection(IEnumerable<Parameter> parameters) {
        foreach (var parameter in parameters) {
            _inner[parameter.Name] = parameter;
        }
    }

    public void Add(Parameter parameter) {
        _inner[parameter.Name] = parameter;
    }

    public IEnumerator<Parameter> GetEnumerator() {
        return _inner.Values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }
}