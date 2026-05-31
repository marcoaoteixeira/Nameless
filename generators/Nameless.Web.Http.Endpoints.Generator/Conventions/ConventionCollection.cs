using System.Collections;

namespace Nameless.Web.Http.Endpoints.Generator.Conventions;

public class ConventionCollection : IEnumerable<Convention> {
    private readonly HashSet<Convention> _conventions;

    public int Count => _conventions.Count;

    public ConventionCollection() {
        _conventions = [];
    }

    public ConventionCollection(IEnumerable<Convention> collection) {
        _conventions = new HashSet<Convention>(collection);
    }

    public void Add(Convention convention) {
        _conventions.Add(convention);
    }

    public IEnumerator<Convention> GetEnumerator() {
        return _conventions.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }
}
