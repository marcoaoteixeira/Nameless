using System.Reflection;
using Nameless.Testing.Tools.Attributes;

namespace Nameless.Web.Http.Endpoints.Attributes.Versioning;

[UnitTest]
public sealed class DeprecateAttributeTests {
    [Fact]
    public void WhenMessageIsSet_ThenMessageIsStored() {
        var attr = new DeprecateAttribute { Message = "Use /v2/users instead." };

        Assert.Equal("Use /v2/users instead.", attr.Message);
    }

    [Fact]
    public void WhenMessageIsNotSet_ThenMessageIsNull() {
        var attr = new DeprecateAttribute();

        Assert.Null(attr.Message);
    }

    [Fact]
    public void WhenSunsetIsSet_ThenSunsetIsStored() {
        var attr = new DeprecateAttribute { Sunset = "Wed, 31 Dec 2025 00:00:00 GMT" };

        Assert.Equal("Wed, 31 Dec 2025 00:00:00 GMT", attr.Sunset);
    }

    [Fact]
    public void WhenSunsetIsNotSet_ThenSunsetIsNull() {
        var attr = new DeprecateAttribute();

        Assert.Null(attr.Sunset);
    }

    [Fact]
    public void WhenInspectingAttributeProperties_ThenAllowMultipleIsFalse() {
        var attr = typeof(DeprecateAttribute);
        var usage = attr.GetCustomAttribute<AttributeUsageAttribute>(inherit: false);

        Assert.NotNull(usage);
        Assert.False(usage.AllowMultiple);
    }

    [Fact]
    public void WhenInspectingAttributeProperties_ThenTargetsClass() {
        var attr = typeof(DeprecateAttribute);
        var usage = attr.GetCustomAttribute<AttributeUsageAttribute>(inherit: false);

        Assert.NotNull(usage);
        Assert.True(usage.ValidOn.HasFlag(AttributeTargets.Class));
    }

    [Fact]
    public void WhenInspectingAttributeProperties_ThenInheritedIsFalse() {
        var attr = typeof(DeprecateAttribute);
        var usage = attr.GetCustomAttribute<AttributeUsageAttribute>(inherit: false);

        Assert.NotNull(usage);
        Assert.False(usage.Inherited);
    }
}
