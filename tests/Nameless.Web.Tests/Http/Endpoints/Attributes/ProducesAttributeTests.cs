namespace Nameless.Web.Http.Endpoints.Attributes;

public sealed class ProducesAttributeTests
{
    [Fact]
    public void ProducesAttribute_WhenCreatedWithType_ThenResponseTypeIsStored()
    {
        var attr = new ProducesAttribute(typeof(string));

        Assert.Equal(typeof(string), attr.ResponseType);
        Assert.Equal(200, attr.StatusCode);
    }

    [Fact]
    public void ProducesAttribute_WhenCreated_ThenContentTypeDefaultsToApplicationJson()
    {
        var attr = new ProducesAttribute(typeof(string));

        Assert.Equal("application/json", attr.ContentType);
    }

    [Fact]
    public void ProducesAttribute_WhenContentTypeIsSet_ThenContentTypeIsStored()
    {
        var attr = new ProducesAttribute(typeof(string)) { ContentType = "application/json" };

        Assert.Equal("application/json", attr.ContentType);
    }

    [Fact]
    public void ProducesAttributeGeneric_WhenCreated_ThenResponseTypeMatchesTypeParam()
    {
        var attr = new ProducesAttribute<string>();

        Assert.Equal(typeof(string), attr.ResponseType);
        Assert.Equal(200, attr.StatusCode);
    }

    [Fact]
    public void ProducesAttribute_WhenInspected_ThenAllowMultipleIsTrue()
    {
        var usage = (AttributeUsageAttribute)typeof(ProducesAttribute)
            .GetCustomAttributes(typeof(AttributeUsageAttribute), inherit: false)
            .Single();

        Assert.True(usage.AllowMultiple);
    }

    [Fact]
    public void ProducesProblemAttribute_WhenCreated_ThenStatusCodeIsStored()
    {
        var attr = new ProducesProblemAttribute();

        Assert.Equal(500, attr.StatusCode);
    }

    [Fact]
    public void ProducesProblemAttribute_WhenInspected_ThenAllowMultipleIsTrue()
    {
        var usage = (AttributeUsageAttribute)typeof(ProducesProblemAttribute)
            .GetCustomAttributes(typeof(AttributeUsageAttribute), inherit: false)
            .Single();

        Assert.True(usage.AllowMultiple);
    }

    [Fact]
    public void ProducesValidationProblemAttribute_WhenCreated_ThenStatusCodeIsStored()
    {
        var attr = new ProducesValidationProblemAttribute();

        Assert.Equal(400, attr.StatusCode);
    }
}
