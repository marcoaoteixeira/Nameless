using System.Collections;
using Nameless.Web.Http.Endpoints.Generator.Emitters;

namespace Nameless.Web.Http.Endpoints.Generator.Models;

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

    public void Add(EndpointGroupModel model) {
        var key = model.Class.FullName;

        if (!_dictionary.ContainsKey(key)) {
            _dictionary.Add(key, model);
        }
    }

    public IEnumerator<EndpointGroupModel> GetEnumerator() {
        return _dictionary.Values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }
}