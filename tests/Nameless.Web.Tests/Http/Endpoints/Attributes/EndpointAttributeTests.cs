using System.Diagnostics.CodeAnalysis;

namespace Nameless.Web.Http.Endpoints.Attributes;

public sealed class EndpointAttributeTests
{
    [Theory]
    [InlineData(typeof(Get),     "GET")]
    [InlineData(typeof(Post),    "POST")]
    [InlineData(typeof(Put),     "PUT")]
    [InlineData(typeof(Patch),   "PATCH")]
    [InlineData(typeof(Delete),  "DELETE")]
    [InlineData(typeof(Head),    "HEAD")]
    [InlineData(typeof(Options), "OPTIONS")]
    public void HttpVerb_Method_ReturnsCorrectHttpMethodString(Type verbType, string expected)
    {
        var method = (string)verbType.GetProperty("Method")!.GetValue(null)!;

        Assert.Equal(expected, method);
    }

    [Theory]
    [InlineData(typeof(Get))]
    [InlineData(typeof(Post))]
    [InlineData(typeof(Put))]
    [InlineData(typeof(Patch))]
    [InlineData(typeof(Delete))]
    [InlineData(typeof(Head))]
    [InlineData(typeof(Options))]
    public void HttpVerb_WhenInspected_ThenImplementsIHttpVerb(Type verbType)
    {
        Assert.True(typeof(IHttpVerb).IsAssignableFrom(verbType));
    }

    [Fact]
    public void EndpointAttribute_WhenCreatedWithRoute_ThenRouteIsStored()
    {
        var attr = new EndpointAttribute<Get>("/users");

        Assert.Equal("/users", attr.Route);
    }

    [Fact]
    public void EndpointAttribute_WhenCreatedWithoutRoute_ThenRouteIsEmpty()
    {
        var attr = new EndpointAttribute<Get>();

        Assert.Equal(string.Empty, attr.Route);
    }

    [Fact]
    public void EndpointAttribute_WhenNameIsSet_ThenNameIsStored()
    {
        var attr = new EndpointAttribute<Get>("/users") { Name = "GetUsers" };

        Assert.Equal("GetUsers", attr.Name);
    }

    [Fact]
    public void EndpointAttribute_WhenNameIsNotSet_ThenNameIsNull()
    {
        var attr = new EndpointAttribute<Get>("/users");

        Assert.Null(attr.Name);
    }

    [Fact]
    public void EndpointAttribute_WhenTagsAreSet_ThenTagsAreStored()
    {
        var attr = new EndpointAttribute<Post>("/orders") { Tags = ["Orders", "Api"] };

        Assert.Equal(["Orders", "Api"], attr.Tags);
    }

    [Fact]
    public void EndpointAttribute_WhenTagsAreNotSet_ThenTagsIsNull()
    {
        var attr = new EndpointAttribute<Post>("/orders");

        Assert.Null(attr.Tags);
    }

    [Fact]
    public void EndpointAttribute_WhenInspected_ThenAllowMultipleIsFalse()
    {
        var usage = (AttributeUsageAttribute)typeof(EndpointAttribute<Get>)
            .GetCustomAttributes(typeof(AttributeUsageAttribute), inherit: false)
            .Single();

        Assert.False(usage.AllowMultiple);
    }

    [Fact]
    public void EndpointAttribute_WhenInspected_ThenTargetsClass()
    {
        var usage = (AttributeUsageAttribute)typeof(EndpointAttribute<Get>)
            .GetCustomAttributes(typeof(AttributeUsageAttribute), inherit: false)
            .Single();

        Assert.True(usage.ValidOn.HasFlag(AttributeTargets.Class));
    }

    [Fact]
    public void EndpointAttribute_WhenInspected_ThenInheritedIsFalse()
    {
        var usage = (AttributeUsageAttribute)typeof(EndpointAttribute<Get>)
            .GetCustomAttributes(typeof(AttributeUsageAttribute), inherit: false)
            .Single();

        Assert.False(usage.Inherited);
    }

    [Fact]
    public void EndpointAttribute_RouteConstructorParam_HasStringSyntaxRouteAttribute()
    {
        var ctor = typeof(EndpointAttribute<Get>).GetConstructors().Single();
        var param = ctor.GetParameters().Single();
        var attr = param.GetCustomAttributes(typeof(StringSyntaxAttribute), inherit: false)
            .Cast<StringSyntaxAttribute>()
            .SingleOrDefault();

        Assert.NotNull(attr);
        Assert.Equal("Route", attr.Syntax);
    }
}
