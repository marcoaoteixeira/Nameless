using System.Reflection;
using Nameless.Testing.Tools.Attributes;

namespace Nameless.Web.Http.Endpoints.Attributes.Produces;

[UnitTest]
public sealed class ProducesResponseAttributeTests {
    [Fact]
    public void WhenCreatedWithType_ThenResponseTypeIsStored() {
        var attr = new ProducesResponseAttribute(typeof(string));

        Assert.Equal(typeof(string), attr.ResponseType);
        Assert.Equal(200, attr.StatusCode);
    }

    [Fact]
    public void WhenCreated_ThenContentTypeDefaultsToApplicationJson() {
        var attr = new ProducesResponseAttribute(typeof(string));

        Assert.Equal("application/json", attr.ContentType);
    }

    [Fact]
    public void WhenCreated_ThenStatusCodeDefaultsTo200Ok() {
        var attr = new ProducesResponseAttribute(typeof(string));

        Assert.Equal(200, attr.StatusCode);
    }

    [Fact]
    public void WhenContentTypeIsSet_ThenContentTypeIsStored() {
        var attr = new ProducesResponseAttribute(typeof(string)) {
            StatusCode = 200,
            ContentType = "application/other+json"
        };

        Assert.Equal("application/other+json", attr.ContentType);
    }

    [Fact]
    public void WhenStatusCodeIsSet_ThenStatusCodeIsStored() {
        var attr = new ProducesResponseAttribute(typeof(string)) { StatusCode = 201 };

        Assert.Equal(201, attr.StatusCode);
    }

    [Fact]
    public void WhenCreatedGeneric_ThenResponseTypeMatchesTypeParam() {
        var attr = new ProducesResponseAttribute<string>();

        Assert.Equal(typeof(string), attr.ResponseType);
        Assert.Equal(200, attr.StatusCode);
    }

    [Fact]
    public void WhenInspectingAttributeProperties_ThenAllowMultipleIsTrue() {
        var attr = typeof(ProducesResponseAttribute);
        var usage = attr.GetCustomAttribute<AttributeUsageAttribute>(inherit: false);

        Assert.NotNull(usage);
        Assert.True(usage.AllowMultiple);
    }

    [Fact]
    public void WhenInspectingAttributeProperties_ThenTargetsClass() {
        var attr = typeof(ProducesResponseAttribute);
        var usage = attr.GetCustomAttribute<AttributeUsageAttribute>(inherit: false);

        Assert.NotNull(usage);
        Assert.True(usage.ValidOn.HasFlag(AttributeTargets.Class));
    }

    [Fact]
    public void WhenInspectingAttributeProperties_ThenInheritedIsFalse() {
        var attr = typeof(ProducesResponseAttribute);
        var usage = attr.GetCustomAttribute<AttributeUsageAttribute>(inherit: false);

        Assert.NotNull(usage);
        Assert.False(usage.Inherited);
    }

    [Fact]
    public void WhenInspectingGenericAttributeProperties_ThenAllowMultipleIsTrue() {
        var attr = typeof(ProducesResponseAttribute<>);
        var usage = attr.GetCustomAttribute<AttributeUsageAttribute>(inherit: false);

        Assert.NotNull(usage);
        Assert.True(usage.AllowMultiple);
    }

    [Fact]
    public void WhenInspectingGenericAttributeProperties_ThenTargetsClass() {
        var attr = typeof(ProducesResponseAttribute<>);
        var usage = attr.GetCustomAttribute<AttributeUsageAttribute>(inherit: false);

        Assert.NotNull(usage);
        Assert.True(usage.ValidOn.HasFlag(AttributeTargets.Class));
    }

    [Fact]
    public void WhenInspectingGenericAttributeProperties_ThenInheritedIsFalse() {
        var attr = typeof(ProducesResponseAttribute<>);
        var usage = attr.GetCustomAttribute<AttributeUsageAttribute>(inherit: false);

        Assert.NotNull(usage);
        Assert.False(usage.Inherited);
    }
}
