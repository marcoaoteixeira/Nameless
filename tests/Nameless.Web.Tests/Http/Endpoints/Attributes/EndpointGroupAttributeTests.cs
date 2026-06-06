using System.Reflection;
using Nameless.Testing.Tools.Attributes;

namespace Nameless.Web.Http.Endpoints.Attributes;

[UnitTest]
public sealed class EndpointGroupAttributeTests {
    [Fact]
    public void WhenDescriptionIsSet_ThenDescriptionIsStored() {
        var attr = new EndpointGroupAttribute("/api/users") { Description = "Get user endpoint" };

        Assert.Equal("Get user endpoint", attr.Description);
    }

    [Fact]
    public void WhenDescriptionIsNotSet_ThenDescriptionIsNull() {
        var attr = new EndpointGroupAttribute("/api/users");

        Assert.Null(attr.Description);
    }

    [Fact]
    public void WhenSummaryIsSet_ThenSummaryIsStored() {
        var attr = new EndpointGroupAttribute("/api/users") { Summary = "Get user endpoint" };

        Assert.Equal("Get user endpoint", attr.Summary);
    }

    [Fact]
    public void WhenSummaryIsNotSet_ThenSummaryIsNull() {
        var attr = new EndpointGroupAttribute("/api/users");

        Assert.Null(attr.Summary);
    }

    [Fact]
    public void WhenInspectingAttributeProperties_ThenAllowMultipleIsFalse() {
        var attr = typeof(EndpointGroupAttribute);
        var usage = attr.GetCustomAttribute<AttributeUsageAttribute>(inherit: false);

        Assert.NotNull(usage);
        Assert.False(usage.AllowMultiple);
    }

    [Fact]
    public void WhenInspectingAttributeProperties_ThenTargetsClass() {
        var attr = typeof(EndpointGroupAttribute);
        var usage = attr.GetCustomAttribute<AttributeUsageAttribute>(inherit: false);

        Assert.NotNull(usage);
        Assert.True(usage.ValidOn.HasFlag(AttributeTargets.Class));
    }

    [Fact]
    public void WhenInspectingAttributeProperties_ThenInheritedIsFalse() {
        var attr = typeof(EndpointGroupAttribute);
        var usage = attr.GetCustomAttribute<AttributeUsageAttribute>(inherit: false);

        Assert.NotNull(usage);
        Assert.False(usage.Inherited);
    }
}
