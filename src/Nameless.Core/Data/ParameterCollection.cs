using System.Collections;

namespace Nameless.Data;

/// <summary>
///     Represents a collection of <see cref="Parameter"/>
/// </summary>
public class ParameterCollection : IEnumerable<Parameter> {
    private readonly Dictionary<string, Parameter> _inner = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    ///     Initializes a new instance of
    ///     <see cref="ParameterCollection"/> class.
    /// </summary>
    public ParameterCollection() { }

    /// <summary>
    ///     Initializes a new instance of
    ///     <see cref="ParameterCollection"/> class.
    /// </summary>
    /// <param name="collection">
    ///     The initialization collection.
    /// </param>
    public ParameterCollection(IEnumerable<Parameter> collection) {
        foreach (var parameter in collection) {
            _inner[parameter.Name] = parameter;
        }
    }

    /// <summary>
    ///     Adds a parameter to the collection, if the parameter exists
    ///     it gets replaced.
    /// </summary>
    /// <param name="parameter">
    ///     The parameter.
    /// </param>
    public void Add(Parameter parameter) {
        _inner[parameter.Name] = parameter;
    }

    /// <inheritdoc />
    public IEnumerator<Parameter> GetEnumerator() {
        return _inner.Values.GetEnumerator();
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }
}