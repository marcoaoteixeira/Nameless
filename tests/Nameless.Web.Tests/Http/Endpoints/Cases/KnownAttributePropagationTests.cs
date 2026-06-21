using Nameless.Web.Http.Endpoints.Infrastructure;

namespace Nameless.Web.Http.Endpoints.Cases;

public sealed class KnownAttributePropagationTests
{
    private static string BuildSource(string extraAttributes, string handleAsyncBody = "Results.Ok()") {
        return $$"""
                 using Nameless.Web.Http.Endpoints.Attributes;
                 using Microsoft.AspNetCore.Http;
                 using Microsoft.AspNetCore.Authorization;
                 using Microsoft.AspNetCore.Cors;
                 using Microsoft.AspNetCore.RateLimiting;
                 using Microsoft.AspNetCore.OutputCaching;
                 using Microsoft.AspNetCore.Http.Timeouts;
                 using System.Threading.Tasks;

                 namespace TestApp;

                 [Endpoint("/test")]
                 {{extraAttributes}}
                 public partial class TestEndpoint
                 {
                     public async Task<IResult> HandleAsync() => {{handleAsyncBody}};
                 }
                 """;
    }

    [Fact]
    public void Generate_WhenAuthorizeAttribute_ThenEmitsRequireAuthorization()
    {
        var source = BuildSource("[Authorize]");

        var generated = GeneratorTestHelper.GetGeneratedSource(source);

        Assert.Contains(".RequireAuthorization()", generated);
    }

    [Fact]
    public void Generate_WhenAuthorizeAttributeWithPolicy_ThenEmitsRequireAuthorizationWithPolicy()
    {
        var source = BuildSource("[Authorize(\"AdminPolicy\")]");

        var generated = GeneratorTestHelper.GetGeneratedSource(source);

        Assert.Contains(".RequireAuthorization(\"AdminPolicy\")", generated);
    }

    [Fact]
    public void Generate_WhenAllowAnonymousAttribute_ThenEmitsAllowAnonymous()
    {
        var source = BuildSource("[AllowAnonymous]");

        var generated = GeneratorTestHelper.GetGeneratedSource(source);

        Assert.Contains(".AllowAnonymous()", generated);
    }

    [Fact]
    public void Generate_WhenEnableCorsAttribute_ThenEmitsRequireCors()
    {
        var source = BuildSource("[EnableCors(\"MyPolicy\")]");

        var generated = GeneratorTestHelper.GetGeneratedSource(source);

        Assert.Contains(".RequireCors(\"MyPolicy\")", generated);
    }

    [Fact]
    public void Generate_WhenEnableRateLimitingAttribute_ThenEmitsRequireRateLimiting()
    {
        var source = BuildSource("[EnableRateLimiting(\"sliding\")]");

        var generated = GeneratorTestHelper.GetGeneratedSource(source);

        Assert.Contains(".RequireRateLimiting(\"sliding\")", generated);
    }

    [Fact]
    public void Generate_WhenOutputCacheAttribute_ThenEmitsCacheOutput()
    {
        var source = BuildSource("[OutputCache(PolicyName = \"default\")]");

        var generated = GeneratorTestHelper.GetGeneratedSource(source);

        Assert.Contains(".CacheOutput(\"default\")", generated);
    }

    [Fact]
    public void Generate_WhenRequestTimeoutAttribute_ThenEmitsWithRequestTimeout()
    {
        var source = BuildSource("[RequestTimeout(\"short\")]");

        var generated = GeneratorTestHelper.GetGeneratedSource(source);

        Assert.Contains(".WithRequestTimeout(\"short\")", generated);
    }

    [Fact]
    public void Generate_WhenSummaryAttribute_ThenEmitsWithSummary()
    {
        const string source = """
            using Nameless.Web.Http.Endpoints.Attributes;
            using Microsoft.AspNetCore.Http;
            using System.Threading.Tasks;

            namespace TestApp;

            [Endpoint("/test")]
            [EndpointSummary("Returns all items")]
            public partial class TestEndpoint
            {
                public async Task<IResult> HandleAsync() => Results.Ok();
            }
            """;

        var generated = GeneratorTestHelper.GetGeneratedSource(source);

        Assert.Contains(".WithSummary(\"Returns all items\")", generated);
    }

    [Fact]
    public void Generate_WhenDescriptionAttribute_ThenEmitsWithDescription()
    {
        const string source = """
            using Nameless.Web.Http.Endpoints.Attributes;
            using Microsoft.AspNetCore.Http;
            using System.Threading.Tasks;

            namespace TestApp;

            [Endpoint("/test")]
            [EndpointDescription("Detailed description here")]
            public partial class TestEndpoint
            {
                public async Task<IResult> HandleAsync() => Results.Ok();
            }
            """;

        var generated = GeneratorTestHelper.GetGeneratedSource(source);

        Assert.Contains(".WithDescription(\"Detailed description here\")", generated);
    }

    [Fact]
    public void Generate_WhenSummaryContainsQuote_ThenStringIsEscapedInGeneratedCode()
    {
        const string source = """
            using Nameless.Web.Http.Endpoints.Attributes;
            using Microsoft.AspNetCore.Http;
            using System.Threading.Tasks;

            namespace TestApp;

            [Endpoint("/test")]
            [EndpointSummary("Say \"hello\"")]
            public partial class TestEndpoint
            {
                public async Task<IResult> HandleAsync() => Results.Ok();
            }
            """;

        var generated = GeneratorTestHelper.GetGeneratedSource(source);

        Assert.Contains(".WithSummary(\"Say \\\"hello\\\"\")", generated);
    }

    [Fact]
    public void Generate_WhenRequireAntiforgeryTokenAttribute_ThenEmitsRequireAntiforgery()
    {
        const string source = """
            using Nameless.Web.Http.Endpoints.Attributes;
            using Microsoft.AspNetCore.Http;
            using Microsoft.AspNetCore.Antiforgery;
            using System.Threading.Tasks;

            namespace TestApp;

            [Endpoint("/test")]
            [RequireAntiforgeryToken]
            public partial class TestEndpoint
            {
                public async Task<IResult> HandleAsync() => Results.Ok();
            }
            """;

        var generated = GeneratorTestHelper.GetGeneratedSource(source);

        Assert.Contains(".RequireAntiforgery()", generated);
    }

    [Fact]
    public void Generate_WhenRequireAntiforgeryTokenFalse_ThenEmitsDisableAntiforgery()
    {
        const string source = """
            using Nameless.Web.Http.Endpoints.Attributes;
            using Microsoft.AspNetCore.Http;
            using Microsoft.AspNetCore.Antiforgery;
            using System.Threading.Tasks;

            namespace TestApp;

            [Endpoint("/test")]
            [RequireAntiforgeryToken(false)]
            public partial class TestEndpoint
            {
                public async Task<IResult> HandleAsync() => Results.Ok();
            }
            """;

        var generated = GeneratorTestHelper.GetGeneratedSource(source);

        Assert.Contains(".DisableAntiforgery()", generated);
    }

    [Fact]
    public void Generate_WhenDisableHttpMetricsAttribute_ThenEmitsDisableHttpMetrics()
    {
        const string source = """
            using Nameless.Web.Http.Endpoints.Attributes;
            using Microsoft.AspNetCore.Http;
            using System.Threading.Tasks;

            namespace TestApp;

            [Endpoint("/test")]
            [DisableHttpMetrics]
            public partial class TestEndpoint
            {
                public async Task<IResult> HandleAsync() => Results.Ok();
            }
            """;

        var generated = GeneratorTestHelper.GetGeneratedSource(source);

        Assert.Contains(".DisableHttpMetrics()", generated);
    }

    [Fact]
    public void Generate_WhenAcceptsAttribute_ThenEmitsAccepts()
    {
        const string source = """
            using Nameless.Web.Http.Endpoints.Attributes;
            using Microsoft.AspNetCore.Http;
            using System.Threading.Tasks;

            namespace TestApp;

            public record CreateRequest(string Name);

            [Endpoint("/test", Verb = HttpVerbs.Post)]
            [Accepts<CreateRequest>("application/json")]
            public partial class TestEndpoint
            {
                public async Task<IResult> HandleAsync() => Results.Ok();
            }
            """;

        var generated = GeneratorTestHelper.GetGeneratedSource(source);

        Assert.Contains(".Accepts<", generated);
        Assert.Contains("CreateRequest", generated);
    }
}
