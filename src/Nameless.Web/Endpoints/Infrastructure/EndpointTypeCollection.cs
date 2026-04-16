using System.Collections;

namespace Nameless.Web.Endpoints.Infrastructure;

/// <summary>
///     Represents a collection of endpoint types.
/// </summary>
internal class EndpointTypeCollection : IEnumerable<Type> {
    private readonly Type[] _endpoints;

    /// <summary>
    ///     Initializes a new instance of the
    ///     <see cref="EndpointTypeCollection"/> class.
    /// </summary>
    /// <param name="endpoints">
    ///     A collection of endpoint types to include in the collection.
    /// </param>
    internal EndpointTypeCollection(IEnumerable<Type> endpoints) {
        _endpoints = [.. endpoints];
    }

    /// <inheritdoc />
    public IEnumerator<Type> GetEnumerator() {
        return ((IEnumerable<Type>)_endpoints).GetEnumerator();
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }
}