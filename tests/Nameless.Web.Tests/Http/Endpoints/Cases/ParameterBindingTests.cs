using Nameless.Web.Http.Endpoints.Infrastructure;

namespace Nameless.Web.Http.Endpoints.Cases;

public sealed class ParameterBindingTests
{
    private static string BuildSource(string handleAsyncParams, string handleAsyncBody = "Results.Ok()") {
        return $$"""
                 using Nameless.Web.Http.Endpoints.Attributes;
                 using Microsoft.AspNetCore.Http;
                 using Microsoft.AspNetCore.Mvc;
                 using System.Threading;
                 using System.Threading.Tasks;

                 namespace TestApp;

                 public record CreateRequest(string Name);
                 public record SearchQuery(int Page);

                 [Endpoint<Post>("/test")]
                 public partial class TestEndpoint
                 {
                     public async Task<IResult> HandleAsync({{handleAsyncParams}}) => {{handleAsyncBody}};
                 }
                 """;
    }

    [Fact]
    public void Generate_WhenFromBodyAttribute_ThenEmitsFromBodyOnLambdaParam()
    {
        var source = BuildSource("[FromBody] CreateRequest body");

        var generated = GeneratorTestHelper.GetGeneratedSource(source);

        Assert.Contains("[global::Microsoft.AspNetCore.Mvc.FromBody]", generated);
    }

    [Fact]
    public void Generate_WhenFromRouteAttribute_ThenEmitsFromRouteOnLambdaParam()
    {
        var source = BuildSource("[FromRoute] int id");

        var generated = GeneratorTestHelper.GetGeneratedSource(source);

        Assert.Contains("[global::Microsoft.AspNetCore.Mvc.FromRoute] int id", generated);
    }

    [Fact]
    public void Generate_WhenFromRouteAttributeWithName_ThenEmitsFromRouteWithNameOnLambdaParam()
    {
        var source = BuildSource("[FromRoute(Name = \"productId\")] int id");

        var generated = GeneratorTestHelper.GetGeneratedSource(source);

        Assert.Contains("FromRoute(Name = \"productId\")", generated);
    }

    [Fact]
    public void Generate_WhenFromQueryAttribute_ThenEmitsFromQueryOnLambdaParam()
    {
        var source = BuildSource("[FromQuery] int page");

        var generated = GeneratorTestHelper.GetGeneratedSource(source);

        Assert.Contains("[global::Microsoft.AspNetCore.Mvc.FromQuery] int page", generated);
    }

    [Fact]
    public void Generate_WhenFromQueryAttributeWithName_ThenEmitsFromQueryWithNameOnLambdaParam()
    {
        var source = BuildSource("[FromQuery(Name = \"page_number\")] int page");

        var generated = GeneratorTestHelper.GetGeneratedSource(source);

        Assert.Contains("FromQuery(Name = \"page_number\")", generated);
    }

    [Fact]
    public void Generate_WhenFromHeaderAttribute_ThenEmitsFromHeaderOnLambdaParam()
    {
        var source = BuildSource("[FromHeader] string correlationId");

        var generated = GeneratorTestHelper.GetGeneratedSource(source);

        Assert.Contains("[global::Microsoft.AspNetCore.Mvc.FromHeader] string correlationId", generated);
    }

    [Fact]
    public void Generate_WhenFromHeaderAttributeWithName_ThenEmitsFromHeaderWithNameOnLambdaParam()
    {
        var source = BuildSource("[FromHeader(Name = \"X-Correlation-Id\")] string correlationId");

        var generated = GeneratorTestHelper.GetGeneratedSource(source);

        Assert.Contains("FromHeader(Name = \"X-Correlation-Id\")", generated);
    }

    [Fact]
    public void Generate_WhenAsParametersAttribute_ThenEmitsAsParametersOnLambdaParam()
    {
        var source = BuildSource("[AsParameters] SearchQuery query");

        var generated = GeneratorTestHelper.GetGeneratedSource(source);

        Assert.Contains("[global::Microsoft.AspNetCore.Http.AsParameters] global::TestApp.SearchQuery query", generated);
    }

    [Fact]
    public void Generate_WhenNoBindingAttribute_ThenNoAttributePrefixEmitted()
    {
        var source = BuildSource("int id");

        var generated = GeneratorTestHelper.GetGeneratedSource(source);

        Assert.DoesNotContain("[global::Microsoft.AspNetCore.Mvc.FromRoute]", generated);
        Assert.DoesNotContain("[global::Microsoft.AspNetCore.Mvc.FromQuery]", generated);
        Assert.DoesNotContain("[global::Microsoft.AspNetCore.Mvc.FromHeader]", generated);
        Assert.DoesNotContain("[global::Microsoft.AspNetCore.Mvc.FromBody]", generated);
        Assert.DoesNotContain("[global::Microsoft.AspNetCore.Http.AsParameters]", generated);
        Assert.Contains("int id", generated);
    }

    [Fact]
    public void Generate_WhenNonBindingAttributePrecedesBindingAttribute_ThenBindingIsStillEmitted()
    {
        // Verifies the binding-attribute loop skips non-binding attributes (e.g. [Required])
        // rather than stopping at the first non-match.
        const string source = """
            using Nameless.Web.Http.Endpoints.Attributes;
            using Microsoft.AspNetCore.Http;
            using Microsoft.AspNetCore.Mvc;
            using System.ComponentModel.DataAnnotations;
            using System.Threading.Tasks;

            namespace TestApp;

            [Endpoint<Get>("/test")]
            public partial class TestEndpoint
            {
                public async Task<IResult> HandleAsync([Required][FromQuery] int page) => Results.Ok();
            }
            """;

        var generated = GeneratorTestHelper.GetGeneratedSource(source);

        Assert.Contains("[global::Microsoft.AspNetCore.Mvc.FromQuery] int page", generated);
    }

    [Fact]
    public void Generate_WhenFromQueryNameContainsQuote_ThenNameIsEscapedInGeneratedCode()
    {
        var source = BuildSource("[FromQuery(Name = \"it\\\"self\")] string val");

        var generated = GeneratorTestHelper.GetGeneratedSource(source);

        Assert.Contains("Name = \"it\\\"self\"", generated);
    }

    [Fact]
    public void Generate_WhenMixedBindingAttributes_ThenAllParamsEmittedCorrectly()
    {
        var source = BuildSource("[FromRoute] int id, [FromQuery] int page, [FromBody] CreateRequest body, CancellationToken ct");

        var generated = GeneratorTestHelper.GetGeneratedSource(source);

        Assert.Contains("[global::Microsoft.AspNetCore.Mvc.FromRoute]", generated);
        Assert.Contains("[global::Microsoft.AspNetCore.Mvc.FromQuery]", generated);
        Assert.Contains("[global::Microsoft.AspNetCore.Mvc.FromBody]", generated);
        Assert.Contains("global::System.Threading.CancellationToken ct", generated);
    }
}
