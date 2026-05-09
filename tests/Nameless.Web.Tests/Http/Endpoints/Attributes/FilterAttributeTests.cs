using Microsoft.AspNetCore.Http;

namespace Nameless.Web.Http.Endpoints.Attributes;

public sealed class FilterAttributeTests
{
    private sealed class SampleFilter : IEndpointFilter
    {
        public ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next) {
            return next(context);
        }
    }

    [Fact]
    public void FilterAttribute_WhenCreatedWithType_ThenFilterTypeIsStored()
    {
        var attr = new FilterAttribute(typeof(SampleFilter));

        Assert.Equal(typeof(SampleFilter), attr.FilterType);
    }

    [Fact]
    public void FilterAttributeGeneric_WhenCreated_ThenFilterTypeMatchesTypeParam()
    {
        var attr = new FilterAttribute<SampleFilter>();

        Assert.Equal(typeof(SampleFilter), attr.FilterType);
    }

    [Fact]
    public void FilterAttribute_WhenInspected_ThenAllowMultipleIsTrue()
    {
        var usage = (AttributeUsageAttribute)typeof(FilterAttribute)
            .GetCustomAttributes(typeof(AttributeUsageAttribute), inherit: false)
            .Single();

        Assert.True(usage.AllowMultiple);
    }

    [Fact]
    public void FilterAttribute_WhenInspected_ThenTargetsClass()
    {
        var usage = (AttributeUsageAttribute)typeof(FilterAttribute)
            .GetCustomAttributes(typeof(AttributeUsageAttribute), inherit: false)
            .Single();

        Assert.True(usage.ValidOn.HasFlag(AttributeTargets.Class));
    }
}
