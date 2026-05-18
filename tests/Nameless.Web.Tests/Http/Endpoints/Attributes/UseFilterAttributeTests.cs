using Microsoft.AspNetCore.Http;
using Nameless.Testing.Tools.Attributes;
using Nameless.Web.Http.Endpoints.Attributes.Filtering;

namespace Nameless.Web.Http.Endpoints.Attributes;

public sealed class UseFilterAttributeTests
{
    private sealed class SampleFilter : IEndpointFilter
    {
        public ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next) {
            return next(context);
        }
    }

    [UnitTest]
    [Fact]
    public void EndpointFilterAttribute_WhenCreatedWithType_ThenFilterTypeIsStored()
    {
        var attr = new UseFilterAttribute(typeof(SampleFilter));

        Assert.Equal(typeof(SampleFilter), attr.FilterType);
    }

    [UnitTest]
    [Fact]
    public void EndpointFilterAttributeGeneric_WhenCreated_ThenFilterTypeMatchesTypeParam()
    {
        var attr = new UseFilterAttribute<SampleFilter>();

        Assert.Equal(typeof(SampleFilter), attr.FilterType);
    }

    [UnitTest]
    [Fact]
    public void EndpointFilterAttribute_WhenInspected_ThenAllowMultipleIsTrue()
    {
        var usage = (AttributeUsageAttribute)typeof(UseFilterAttribute)
            .GetCustomAttributes(typeof(AttributeUsageAttribute), inherit: false)
            .Single();

        Assert.True(usage.AllowMultiple);
    }

    [UnitTest]
    [Fact]
    public void EndpointFilterAttribute_WhenInspected_ThenTargetsClass()
    {
        var usage = (AttributeUsageAttribute)typeof(UseFilterAttribute)
            .GetCustomAttributes(typeof(AttributeUsageAttribute), inherit: false)
            .Single();

        Assert.True(usage.ValidOn.HasFlag(AttributeTargets.Class));
    }
}
