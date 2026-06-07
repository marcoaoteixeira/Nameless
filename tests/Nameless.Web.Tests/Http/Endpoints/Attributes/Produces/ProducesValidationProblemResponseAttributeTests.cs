using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Nameless.Testing.Tools.Attributes;

namespace Nameless.Web.Http.Endpoints.Attributes.Produces;

[UnitTest]
public sealed class ProducesValidationProblemResponseAttributeTests {
    [Fact]
    public void WhenCreated_ThenResponseTypeIsProblemDetails() {
        var attr = new ProducesValidationProblemResponseAttribute();

        Assert.Equal(typeof(ProblemDetails), attr.ResponseType);
    }

    [Fact]
    public void WhenCreated_ThenContentTypeDefaultsToApplicationProblemJson() {
        var attr = new ProducesValidationProblemResponseAttribute();

        Assert.Equal("application/problem+json", attr.ContentType);
    }

    [Fact]
    public void WhenCreated_ThenStatusCodeDefaultsTo400BadRequest() {
        var attr = new ProducesValidationProblemResponseAttribute();

        Assert.Equal(400, attr.StatusCode);
    }

    [Fact]
    public void WhenContentTypeIsSet_ThenContentTypeIsStored() {
        var attr = new ProducesValidationProblemResponseAttribute {
            ContentType = "application/other+json"
        };

        Assert.Equal("application/other+json", attr.ContentType);
    }

    [Fact]
    public void WhenStatusCodeIsSet_ThenStatusCodeIsStored() {
        var attr = new ProducesValidationProblemResponseAttribute {
            StatusCode = 404
        };

        Assert.Equal(404, attr.StatusCode);
    }

    [Fact]
    public void WhenInspectingAttributeProperties_ThenAllowMultipleIsFalse() {
        var attr = typeof(ProducesValidationProblemResponseAttribute);
        var usage = attr.GetCustomAttribute<AttributeUsageAttribute>(inherit: false);

        Assert.NotNull(usage);
        Assert.False(usage.AllowMultiple);
    }

    [Fact]
    public void WhenInspectingAttributeProperties_ThenTargetsClass() {
        var attr = typeof(ProducesValidationProblemResponseAttribute);
        var usage = attr.GetCustomAttribute<AttributeUsageAttribute>(inherit: false);

        Assert.NotNull(usage);
        Assert.True(usage.ValidOn.HasFlag(AttributeTargets.Class));
    }

    [Fact]
    public void WhenInspectingAttributeProperties_ThenInheritedIsFalse() {
        var attr = typeof(ProducesValidationProblemResponseAttribute);
        var usage = attr.GetCustomAttribute<AttributeUsageAttribute>(inherit: false);

        Assert.NotNull(usage);
        Assert.False(usage.Inherited);
    }
}
