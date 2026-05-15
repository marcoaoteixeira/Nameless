using Microsoft.AspNetCore.Http;
using Nameless.Testing.Tools.Attributes;
using Nameless.Web.Http.Endpoints.Attributes;

namespace Nameless.Web.Http.Endpoints.Attributes;

public sealed class EndpointFilterAttributeTests
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
        var attr = new EndpointFilterAttribute(typeof(SampleFilter));

        Assert.Equal(typeof(SampleFilter), attr.FilterType);
    }

    [UnitTest]
    [Fact]
    public void EndpointFilterAttributeGeneric_WhenCreated_ThenFilterTypeMatchesTypeParam()
    {
        var attr = new EndpointFilterAttribute<SampleFilter>();

        Assert.Equal(typeof(SampleFilter), attr.FilterType);
    }

    [UnitTest]
    [Fact]
    public void EndpointFilterAttribute_WhenInspected_ThenAllowMultipleIsTrue()
    {
        var usage = (AttributeUsageAttribute)typeof(EndpointFilterAttribute)
            .GetCustomAttributes(typeof(AttributeUsageAttribute), inherit: false)
            .Single();

        Assert.True(usage.AllowMultiple);
    }

    [UnitTest]
    [Fact]
    public void EndpointFilterAttribute_WhenInspected_ThenTargetsClass()
    {
        var usage = (AttributeUsageAttribute)typeof(EndpointFilterAttribute)
            .GetCustomAttributes(typeof(AttributeUsageAttribute), inherit: false)
            .Single();

        Assert.True(usage.ValidOn.HasFlag(AttributeTargets.Class));
    }
}
