using System.Collections;
using Nameless.Generators.Emitters;
using Nameless.Generators.Models;

namespace Nameless.Generators.Web.Http.Endpoints.Models;

public class EndpointGroupModelCollection : IEnumerable<EndpointGroupModel>, IEmitModel {
    private readonly Dictionary<string, EndpointGroupModel> _dictionary;

    public ClassModel Class { get; } = new() {
        Namespace = Project.Namespaces.Root,
        Name = Project.RegistrationClassName,
        Accessibility = "public"
    };

    public int Count => _dictionary.Count;

    public EndpointGroupModelCollection() {
        _dictionary = [];
    }

    public EndpointGroupModelCollection(IEnumerable<EndpointGroupModel> collection) {
        _dictionary = collection.ToDictionary(
            keySelector: item => item.Class.FullName,
            elementSelector: item => item
        );
    }

    public bool Add(EndpointGroupModel model) {
        var key = model.Class.FullName;

        if (_dictionary.ContainsKey(key)) {
            return false;
        }

        _dictionary.Add(key, model);

        return true;
    }

    public IEnumerator<EndpointGroupModel> GetEnumerator() {
        return _dictionary.Values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }
}