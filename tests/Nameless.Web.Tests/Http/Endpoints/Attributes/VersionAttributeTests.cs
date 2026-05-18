using Nameless.Web.Http.Endpoints.Attributes.Versioning;

namespace Nameless.Web.Http.Endpoints.Attributes;

public sealed class VersionAttributeTests
{
    [Fact]
    public void VersionAttribute_WhenCreatedWithVersion_ThenVersionIsStored()
    {
        var attr = new VersionAttribute("1.0.0");

        Assert.Equal("1.0.0", attr.Version);
    }

    [Fact]
    public void VersionAttribute_WhenCreated_ThenDeprecatedDefaultsToFalse()
    {
        var attr = new VersionAttribute("1.0.0");

        Assert.False(attr.Deprecated);
    }

    [Fact]
    public void VersionAttribute_WhenDeprecatedIsSet_ThenDeprecatedIsTrue()
    {
        var attr = new VersionAttribute("1.0.0") { Deprecated = true };

        Assert.True(attr.Deprecated);
    }

    [Fact]
    public void VersionAttribute_WhenInspected_ThenAllowMultipleIsTrue()
    {
        var usage = (AttributeUsageAttribute)typeof(VersionAttribute)
            .GetCustomAttributes(typeof(AttributeUsageAttribute), inherit: false)
            .Single();

        Assert.True(usage.AllowMultiple);
    }

    [Fact]
    public void VersionAttribute_WhenInspected_ThenTargetsClass()
    {
        var usage = (AttributeUsageAttribute)typeof(VersionAttribute)
            .GetCustomAttributes(typeof(AttributeUsageAttribute), inherit: false)
            .Single();

        Assert.True(usage.ValidOn.HasFlag(AttributeTargets.Class));
    }
}
