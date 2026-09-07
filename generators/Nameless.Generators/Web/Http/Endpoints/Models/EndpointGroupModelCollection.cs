using System.Collections;
using Nameless.Generators.Shared.Emitters;
using Nameless.Generators.Shared.Models;

namespace Nameless.Generators.Web.Http.Endpoints.Models;

public class EndpointGroupModelCollection : IEnumerable<EndpointGroupModel>, IEmitModel {
    private readonly Dictionary<string, EndpointGroupModel> _dictionary;
    private readonly string _assemblyName;
    private readonly Lazy<ClassModel> _classModel;

    public ClassModel Class => _classModel.Value;

    public int Count => _dictionary.Count;

    public EndpointGroupModelCollection(IEnumerable<EndpointGroupModel>? collection = null, string? assemblyName = null) {
        _dictionary = (collection ?? []).ToDictionary(
            keySelector: item => item.Class.FullName,
            elementSelector: item => item
        );

        _assemblyName = assemblyName ?? string.Empty;
        _classModel = new Lazy<ClassModel>(CreateClassModel);
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

    private ClassModel CreateClassModel() {
        // Builds the namespace under which generator-defined types (the registration
        // extensions class, the synthetic catch-all group) are emitted for a given
        // producing assembly, so that the same generator running for two different
        // assemblies never produces two types with an identical full name.

        return new ClassModel {
            Namespace = $"{_assemblyName}.AutoGenCode",
            Name = Project.RegistrationClassName,
            Accessibility = "public"
        };
    }
}