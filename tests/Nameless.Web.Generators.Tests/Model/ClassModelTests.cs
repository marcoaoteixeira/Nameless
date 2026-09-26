using Nameless.Testing.Tools.Attributes;
using Nameless.Web.Generators.Models;

namespace Nameless.Web.Generators.Model;

[UnitTest]
public class ClassModelTests
{
    [Fact]
    public void FullName_WithNamespace_ReturnsDotSeparated()
    {
        var model = new ClassModel {
            Namespace = "My.App",
            Name = "MyClass",
            Accessibility = "public"
        };

        Assert.Equal("My.App.MyClass", model.FullName);
    }

    [Fact]
    public void FullName_EmptyNamespace_ReturnsNameOnly()
    {
        var model = new ClassModel {
            Name = "MyClass",
            Accessibility = "public"
        };

        Assert.Equal("MyClass", model.FullName);
    }
}
