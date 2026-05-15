using Nameless.Testing.Tools.Attributes;
using Nameless.Web.Http.Endpoints.Attributes;

namespace Nameless.Web.Http.Endpoints.Attributes;

public sealed class EndpointGroupingAttributeTests
{
    [UnitTest]
    [Fact]
    public void EndpointGroupingAttribute_WhenCreated_ThenNameAndPrefixAreStored()
    {
        var attr = new EndpointGroupingAttribute("Users", "/api/users");

        Assert.Equal("Users", attr.Name);
        Assert.Equal("/api/users", attr.Prefix);
    }

    [UnitTest]
    [Fact]
    public void EndpointGroupingAttribute_WhenInspected_ThenAllowMultipleIsFalse()
    {
        var usage = (AttributeUsageAttribute)typeof(EndpointGroupingAttribute)
            .GetCustomAttributes(typeof(AttributeUsageAttribute), inherit: false)
            .Single();

        Assert.False(usage.AllowMultiple);
    }

    [UnitTest]
    [Fact]
    public void EndpointGroupingAttribute_WhenInspected_ThenTargetsClass()
    {
        var usage = (AttributeUsageAttribute)typeof(EndpointGroupingAttribute)
            .GetCustomAttributes(typeof(AttributeUsageAttribute), inherit: false)
            .Single();

        Assert.True(usage.ValidOn.HasFlag(AttributeTargets.Class));
    }

    [UnitTest]
    [Fact]
    public void EndpointGroupingAttribute_WhenVersionsSet_ThenVersionsAreStored()
    {
        var attr = new EndpointGroupingAttribute("Users", "/api/users") { Versions = ["1.0", "2.0"] };

        Assert.Equal(["1.0", "2.0"], attr.Versions);
    }
}
