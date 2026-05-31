using System.Reflection;
using Nameless.Testing.Tools.Attributes;

namespace Nameless.Web.Http.Endpoints.Attributes.RequestTimeout;

[UnitTest]
public sealed class DisableRequestTimeoutAttributeTests {
    [Fact]
    public void WhenInspectingAttributeProperties_ThenAllowMultipleIsFalse() {
        var attr = typeof(DisableRequestTimeoutAttribute);
        var usage = attr.GetCustomAttribute<AttributeUsageAttribute>(inherit: false);

        Assert.NotNull(usage);
        Assert.False(usage.AllowMultiple);
    }

    [Fact]
    public void WhenInspectingAttributeProperties_ThenTargetsClass() {
        var attr = typeof(DisableRequestTimeoutAttribute);
        var usage = attr.GetCustomAttribute<AttributeUsageAttribute>(inherit: false);

        Assert.NotNull(usage);
        Assert.True(usage.ValidOn.HasFlag(AttributeTargets.Class));
    }

    [Fact]
    public void WhenInspectingAttributeProperties_ThenInheritedIsFalse() {
        var attr = typeof(DisableRequestTimeoutAttribute);
        var usage = attr.GetCustomAttribute<AttributeUsageAttribute>(inherit: false);

        Assert.NotNull(usage);
        Assert.False(usage.Inherited);
    }
}
