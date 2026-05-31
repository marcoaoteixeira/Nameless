using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Nameless.Testing.Tools.Attributes;

namespace Nameless.Web.Http.Endpoints.Attributes.Produces;

[UnitTest]
public sealed class ProducesProblemAttributeTests {
    [Fact]
    public void WhenCreated_ThenResponseTypeIsProblemDetails() {
        var attr = new ProducesProblemAttribute();

        Assert.Equal(typeof(ProblemDetails), attr.ResponseType);
    }

    [Fact]
    public void WhenCreated_ThenContentTypeDefaultsToApplicationProblemJson() {
        var attr = new ProducesProblemAttribute();

        Assert.Equal("application/problem+json", attr.ContentType);
    }

    [Fact]
    public void WhenCreated_ThenStatusCodeDefaultsTo500ServerInternalError() {
        var attr = new ProducesProblemAttribute();

        Assert.Equal(500, attr.StatusCode);
    }

    [Fact]
    public void WhenContentTypeIsSet_ThenContentTypeIsStored() {
        var attr = new ProducesProblemAttribute(contentType: "application/other+json");

        Assert.Equal("application/other+json", attr.ContentType);
    }

    [Fact]
    public void WhenStatusCodeIsSet_ThenStatusCodeIsStored() {
        var attr = new ProducesProblemAttribute(501);

        Assert.Equal(501, attr.StatusCode);
    }

    [Fact]
    public void WhenInspectingAttributeProperties_ThenAllowMultipleIsTrue() {
        var attr = typeof(ProducesProblemAttribute);
        var usage = attr.GetCustomAttribute<AttributeUsageAttribute>(inherit: false);

        Assert.NotNull(usage);
        Assert.True(usage.AllowMultiple);
    }

    [Fact]
    public void WhenInspectingAttributeProperties_ThenTargetsClass() {
        var attr = typeof(ProducesProblemAttribute);
        var usage = attr.GetCustomAttribute<AttributeUsageAttribute>(inherit: false);

        Assert.NotNull(usage);
        Assert.True(usage.ValidOn.HasFlag(AttributeTargets.Class));
    }

    [Fact]
    public void WhenInspectingAttributeProperties_ThenInheritedIsFalse() {
        var attr = typeof(ProducesProblemAttribute);
        var usage = attr.GetCustomAttribute<AttributeUsageAttribute>(inherit: false);

        Assert.NotNull(usage);
        Assert.False(usage.Inherited);
    }
}
