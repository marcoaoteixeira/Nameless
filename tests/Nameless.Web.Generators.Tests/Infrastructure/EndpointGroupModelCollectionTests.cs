using Nameless.Testing.Tools.Attributes;
using Nameless.Web.Generators.Models;

namespace Nameless.Web.Generators.Infrastructure;

[UnitTest]
public class EndpointGroupModelCollectionTests
{
    [Fact]
    public void Add_NewGroup_ReturnsTrueAndIncreasesCount()
    {
        var collection = new EndpointGroupModelCollection();

        var added = collection.Add(CreateGroup("Ns", "GroupA"));

        Assert.True(added);
        Assert.Equal(1, collection.Count);
    }

    [Fact]
    public void Add_DuplicateFullName_ReturnsFalseAndCountUnchanged()
    {
        var collection = new EndpointGroupModelCollection { CreateGroup("Ns", "GroupA") };

        var added = collection.Add(CreateGroup("Ns", "GroupA"));

        Assert.False(added);
        Assert.Equal(1, collection.Count);
    }

    [Fact]
    public void Add_SameNameDifferentNamespace_TreatedAsDistinct()
    {
        var collection = new EndpointGroupModelCollection();
        collection.Add(CreateGroup("Ns1", "Group"));
        collection.Add(CreateGroup("Ns2", "Group"));

        Assert.Equal(2, collection.Count);
    }

    [Fact]
    public void Constructor_FromCollection_PopulatesCorrectly()
    {
        var groups = new[]
        {
            CreateGroup("Ns", "A"),
            CreateGroup("Ns", "B")
        };

        var collection = new EndpointGroupModelCollection(groups);

        Assert.Equal(2, collection.Count);
    }

    [Fact]
    public void Enumeration_YieldsAllGroups()
    {
        var collection = new EndpointGroupModelCollection();
        collection.Add(CreateGroup("Ns", "A"));
        collection.Add(CreateGroup("Ns", "B"));

        var names = collection.Select(g => g.Class.Name).ToArray();

        Assert.Contains("A", names);
        Assert.Contains("B", names);
    }

    private static EndpointGroupModel CreateGroup(string ns, string name) {
        return new EndpointGroupModel {
            Class = new ClassModel {
                Namespace = ns,
                Name = name,
                Accessibility = "public"
            },
            Arguments = new EndpointGroupArgumentsModel {
                Prefix = "/prefix"
            },
            Conventions = [],
            Endpoints = [],
            ReportVersions = [],
            Location = default
        };
    }
}
