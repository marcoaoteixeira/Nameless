using System.Reflection;
using Microsoft.AspNetCore.Http;
using Nameless.Testing.Tools.Attributes;

namespace Nameless.Web.Http.Endpoints.Attributes.Filtering;

[UnitTest]
public sealed class UseFilterAttributeTests {
    private sealed class SampleFilter : IEndpointFilter {
        public ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next) {
            return next(context);
        }
    }

    [Fact]
    public void WhenCreatedWithType_ThenFilterTypeIsStored() {
        var attr = new UseFilterAttribute(typeof(SampleFilter));

        Assert.Equal(typeof(SampleFilter), attr.FilterType);
    }

    [Fact]
    public void WhenCreatedGeneric_ThenFilterTypeMatchesTypeParam() {
        var attr = new UseFilterAttribute<SampleFilter>();

        Assert.Equal(typeof(SampleFilter), attr.FilterType);
    }

    [Fact]
    public void WhenInspectingAttributeProperties_ThenAllowMultipleIsTrue() {
        var attr = typeof(UseFilterAttribute);
        var usage = attr.GetCustomAttribute<AttributeUsageAttribute>(inherit: false);

        Assert.NotNull(usage);
        Assert.True(usage.AllowMultiple);
    }

    [Fact]
    public void WhenInspectingAttributeProperties_ThenTargetsClass() {
        var attr = typeof(UseFilterAttribute);
        var usage = attr.GetCustomAttribute<AttributeUsageAttribute>(inherit: false);

        Assert.NotNull(usage);
        Assert.True(usage.ValidOn.HasFlag(AttributeTargets.Class));
    }

    [Fact]
    public void WhenInspectingAttributeProperties_ThenInheritedIsFalse() {
        var attr = typeof(UseFilterAttribute);
        var usage = attr.GetCustomAttribute<AttributeUsageAttribute>(inherit: false);

        Assert.NotNull(usage);
        Assert.False(usage.Inherited);
    }

    [Fact]
    public void WhenInspectingGenericAttributeProperties_ThenAllowMultipleIsTrue() {
        var attr = typeof(UseFilterAttribute<>);
        var usage = attr.GetCustomAttribute<AttributeUsageAttribute>(inherit: false);

        Assert.NotNull(usage);
        Assert.True(usage.AllowMultiple);
    }

    [Fact]
    public void WhenInspectingGenericAttributeProperties_ThenTargetsClass() {
        var attr = typeof(UseFilterAttribute<>);
        var usage = attr.GetCustomAttribute<AttributeUsageAttribute>(inherit: false);

        Assert.NotNull(usage);
        Assert.True(usage.ValidOn.HasFlag(AttributeTargets.Class));
    }

    [Fact]
    public void WhenInspectingGenericAttributeProperties_ThenInheritedIsFalse() {
        var attr = typeof(UseFilterAttribute<>);
        var usage = attr.GetCustomAttribute<AttributeUsageAttribute>(inherit: false);

        Assert.NotNull(usage);
        Assert.False(usage.Inherited);
    }
}
