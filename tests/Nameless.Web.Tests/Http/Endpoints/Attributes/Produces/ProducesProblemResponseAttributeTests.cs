using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Nameless.Testing.Tools.Attributes;

namespace Nameless.Web.Http.Endpoints.Attributes.Produces;

[UnitTest]
public sealed class ProducesProblemResponseAttributeTests {
    [Fact]
    public void WhenCreated_ThenResponseTypeIsProblemDetails() {
        var attr = new ProducesProblemResponseAttribute();

        Assert.Equal(typeof(ProblemDetails), attr.ResponseType);
    }

    [Fact]
    public void WhenCreated_ThenContentTypeDefaultsToApplicationProblemJson() {
        var attr = new ProducesProblemResponseAttribute();

        Assert.Equal("application/problem+json", attr.ContentType);
    }

    [Fact]
    public void WhenCreated_ThenStatusCodeDefaultsTo500ServerInternalError() {
        var attr = new ProducesProblemResponseAttribute();

        Assert.Equal(500, attr.StatusCode);
    }

    [Fact]
    public void WhenContentTypeIsSet_ThenContentTypeIsStored() {
        var attr = new ProducesProblemResponseAttribute {
            ContentType = "application/other+json"
        };

        Assert.Equal("application/other+json", attr.ContentType);
    }

    [Fact]
    public void WhenStatusCodeIsSet_ThenStatusCodeIsStored() {
        var attr = new ProducesProblemResponseAttribute {
            StatusCode = 501
        };

        Assert.Equal(501, attr.StatusCode);
    }

    [Fact]
    public void WhenInspectingAttributeProperties_ThenAllowMultipleIsTrue() {
        var attr = typeof(ProducesProblemResponseAttribute);
        var usage = attr.GetCustomAttribute<AttributeUsageAttribute>(inherit: false);

        Assert.NotNull(usage);
        Assert.True(usage.AllowMultiple);
    }

    [Fact]
    public void WhenInspectingAttributeProperties_ThenTargetsClass() {
        var attr = typeof(ProducesProblemResponseAttribute);
        var usage = attr.GetCustomAttribute<AttributeUsageAttribute>(inherit: false);

        Assert.NotNull(usage);
        Assert.True(usage.ValidOn.HasFlag(AttributeTargets.Class));
    }

    [Fact]
    public void WhenInspectingAttributeProperties_ThenInheritedIsFalse() {
        var attr = typeof(ProducesProblemResponseAttribute);
        var usage = attr.GetCustomAttribute<AttributeUsageAttribute>(inherit: false);

        Assert.NotNull(usage);
        Assert.False(usage.Inherited);
    }
}
