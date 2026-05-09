namespace Nameless.Web.Http.Endpoints.Attributes;

public sealed class AcceptsAttributeTests
{
    [Fact]
    public void AcceptsAttribute_WhenCreatedWithType_ThenRequestTypeIsStored()
    {
        var attr = new AcceptsAttribute(typeof(string));

        Assert.Equal(typeof(string), attr.RequestType);
    }

    [Fact]
    public void AcceptsAttribute_WhenCreated_ThenContentTypeDefaultsToNull()
    {
        var attr = new AcceptsAttribute(typeof(string));

        Assert.Null(attr.ContentType);
    }

    [Fact]
    public void AcceptsAttribute_WhenContentTypeIsSet_ThenContentTypeIsStored()
    {
        var attr = new AcceptsAttribute(typeof(string)) { ContentType = "application/json" };

        Assert.Equal("application/json", attr.ContentType);
    }

    [Fact]
    public void AcceptsAttributeGeneric_WhenCreated_ThenRequestTypeMatchesTypeParam()
    {
        var attr = new AcceptsAttribute<string>();

        Assert.Equal(typeof(string), attr.RequestType);
    }

    [Fact]
    public void AcceptsAttribute_WhenInspected_ThenAllowMultipleIsFalse()
    {
        var usage = (AttributeUsageAttribute)typeof(AcceptsAttribute)
            .GetCustomAttributes(typeof(AttributeUsageAttribute), inherit: false)
            .Single();

        Assert.False(usage.AllowMultiple);
    }

    [Fact]
    public void AcceptsAttribute_WhenInspected_ThenTargetsClass()
    {
        var usage = (AttributeUsageAttribute)typeof(AcceptsAttribute)
            .GetCustomAttributes(typeof(AttributeUsageAttribute), inherit: false)
            .Single();

        Assert.True(usage.ValidOn.HasFlag(AttributeTargets.Class));
    }
}
