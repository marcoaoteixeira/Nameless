using System.Reflection;
using Nameless.Testing.Tools.Attributes;

namespace Nameless.Web.Http.Endpoints.Attributes;

[UnitTest]
public sealed class EndpointAttributeTests {
    [Theory]
    [InlineData(typeof(Get), "GET")]
    [InlineData(typeof(Post), "POST")]
    [InlineData(typeof(Put), "PUT")]
    [InlineData(typeof(Patch), "PATCH")]
    [InlineData(typeof(Delete), "DELETE")]
    [InlineData(typeof(Head), "HEAD")]
    [InlineData(typeof(Options), "OPTIONS")]
    public void WhenGetMethod_ThenReturnsCorrectHttpMethodString(Type verbType, string expected) {
        var method = (string?)verbType.GetProperty("Method")?.GetValue(null);

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
    public void WhenInspectingHttpVerb_ThenImplementsIHttpVerb(Type verbType) {
        Assert.True(typeof(IHttpVerb).IsAssignableFrom(verbType));
    }

    [Fact]
    public void WhenCreatedWithRoute_ThenRouteIsStored() {
        var attr = new EndpointAttribute<Get>("/users");

        Assert.Equal("/users", attr.Route);
    }

    [Fact]
    public void WhenCreatedWithRoute_ThenRouteIsEmpty() {
        var attr = new EndpointAttribute<Get>();

        Assert.Equal(string.Empty, attr.Route);
    }

    [Fact]
    public void WhenNameIsSet_ThenNameIsStored() {
        var attr = new EndpointAttribute<Get>("/users") { Name = "GetUsers" };

        Assert.Equal("GetUsers", attr.Name);
    }

    [Fact]
    public void WhenNameIsNotSet_ThenNameIsNull() {
        var attr = new EndpointAttribute<Get>("/users");

        Assert.Null(attr.Name);
    }

    [Fact]
    public void WhenDescriptionIsSet_ThenDescriptionIsStored() {
        var attr = new EndpointAttribute<Get>("/users") { Description = "Get user endpoint" };

        Assert.Equal("Get user endpoint", attr.Description);
    }

    [Fact]
    public void WhenDescriptionIsNotSet_ThenDescriptionIsNull() {
        var attr = new EndpointAttribute<Get>("/users");

        Assert.Null(attr.Description);
    }

    [Fact]
    public void WhenSummaryIsSet_ThenSummaryIsStored() {
        var attr = new EndpointAttribute<Get>("/users") { Summary = "Get user endpoint" };

        Assert.Equal("Get user endpoint", attr.Summary);
    }

    [Fact]
    public void WhenSummaryIsNotSet_ThenSummaryIsNull() {
        var attr = new EndpointAttribute<Get>("/users");

        Assert.Null(attr.Summary);
    }

    [Fact]
    public void WhenTagsAreSet_ThenTagsAreStored() {
        var attr = new EndpointAttribute<Post>("/orders") { Tags = ["Orders", "Api"] };

        Assert.Equal(["Orders", "Api"], attr.Tags);
    }

    [Fact]
    public void WhenTagsAreNotSet_ThenTagsIsNull() {
        var attr = new EndpointAttribute<Post>("/orders");

        Assert.Null(attr.Tags);
    }

    [Fact]
    public void WhenVersionIsSet_ThenVersionIsStored() {
        var attr = new EndpointAttribute<Get>("/users") { Version = "1.0-alpha" };

        Assert.Equal("1.0-alpha", attr.Version);
    }

    [Fact]
    public void WhenVersionIsNotSet_ThenVersionIsNull() {
        var attr = new EndpointAttribute<Get>("/users");

        Assert.Null(attr.Version);
    }

    [Fact]
    public void WhenGroupIsSet_ThenGroupIsStored() {
        var attr = new EndpointAttribute<Get>("/users") { Group = typeof(int) };

        Assert.Equal(typeof(int), attr.Group);
    }

    [Fact]
    public void WhenGroupIsNotSet_ThenGroupIsNull() {
        var attr = new EndpointAttribute<Get>("/users");

        Assert.Null(attr.Group);
    }

    [Fact]
    public void WhenInspectingAttributeProperties_ThenAllowMultipleIsFalse() {
        var attr = typeof(EndpointAttribute<Get>);
        var usage = attr.GetCustomAttribute<AttributeUsageAttribute>(inherit: false);

        Assert.NotNull(usage);
        Assert.False(usage.AllowMultiple);
    }

    [Fact]
    public void WhenInspectingAttributeProperties_ThenTargetsClass() {
        var attr = typeof(EndpointAttribute<Get>);
        var usage = attr.GetCustomAttribute<AttributeUsageAttribute>(inherit: false);

        Assert.NotNull(usage);
        Assert.True(usage.ValidOn.HasFlag(AttributeTargets.Class));
    }

    [Fact]
    public void WhenInspectingAttributeProperties_ThenInheritedIsFalse() {
        var attr = typeof(EndpointAttribute<Get>);
        var usage = attr.GetCustomAttribute<AttributeUsageAttribute>(inherit: false);

        Assert.NotNull(usage);
        Assert.False(usage.Inherited);
    }
}
