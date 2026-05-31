using System.Reflection;
using Nameless.Testing.Tools.Attributes;

namespace Nameless.Web.Http.Endpoints.Attributes.OutputCache;

[UnitTest]
public sealed class UseOutputCacheAttributeTests {
    [Fact]
    public void WhenCreatedWithPolicyName_ThenPolicyNameIsStored() {
        var attr = new UseOutputCacheAttribute("MyCachePolicy");

        Assert.Equal("MyCachePolicy", attr.PolicyName);
    }

    [Fact]
    public void WhenInspectingAttributeProperties_ThenAllowMultipleIsFalse() {
        var attr = typeof(UseOutputCacheAttribute);
        var usage = attr.GetCustomAttribute<AttributeUsageAttribute>(inherit: false);

        Assert.NotNull(usage);
        Assert.False(usage.AllowMultiple);
    }

    [Fact]
    public void WhenInspectingAttributeProperties_ThenTargetsClass() {
        var attr = typeof(UseOutputCacheAttribute);
        var usage = attr.GetCustomAttribute<AttributeUsageAttribute>(inherit: false);

        Assert.NotNull(usage);
        Assert.True(usage.ValidOn.HasFlag(AttributeTargets.Class));
    }

    [Fact]
    public void WhenInspectingAttributeProperties_ThenInheritedIsFalse() {
        var attr = typeof(UseOutputCacheAttribute);
        var usage = attr.GetCustomAttribute<AttributeUsageAttribute>(inherit: false);

        Assert.NotNull(usage);
        Assert.False(usage.Inherited);
    }
}
