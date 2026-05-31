using System.Reflection;
using Nameless.Testing.Tools.Attributes;

namespace Nameless.Web.Http.Endpoints.Attributes.Cors;

[UnitTest]
public sealed class UseCorsAttributeTests {
    [Fact]
    public void WhenCreatedWithPolicyName_ThenPolicyNameIsStored() {
        var attr = new UseCorsAttribute("MyCorsPolicy");

        Assert.Equal("MyCorsPolicy", attr.PolicyName);
    }

    [Fact]
    public void WhenInspectingAttributeProperties_ThenAllowMultipleIsFalse() {
        var attr = typeof(UseCorsAttribute);
        var usage = attr.GetCustomAttribute<AttributeUsageAttribute>(inherit: false);

        Assert.NotNull(usage);
        Assert.False(usage.AllowMultiple);
    }

    [Fact]
    public void WhenInspectingAttributeProperties_ThenTargetsClass() {
        var attr = typeof(UseCorsAttribute);
        var usage = attr.GetCustomAttribute<AttributeUsageAttribute>(inherit: false);

        Assert.NotNull(usage);
        Assert.True(usage.ValidOn.HasFlag(AttributeTargets.Class));
    }

    [Fact]
    public void WhenInspectingAttributeProperties_ThenInheritedIsFalse() {
        var attr = typeof(UseCorsAttribute);
        var usage = attr.GetCustomAttribute<AttributeUsageAttribute>(inherit: false);

        Assert.NotNull(usage);
        Assert.False(usage.Inherited);
    }
}
