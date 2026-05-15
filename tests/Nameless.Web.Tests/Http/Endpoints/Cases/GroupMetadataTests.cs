using Nameless.Web.Http.Endpoints.Infrastructure;

namespace Nameless.Web.Http.Endpoints.Cases;

public sealed class GroupMetadataTests
{
    private static string BuildGroupSource(string groupAttributes, string extraUsings = "") {
        return $$"""
                 using Nameless.Web.Http.Endpoints.Attributes;
                 using Microsoft.AspNetCore.Http;
                 using Microsoft.AspNetCore.Authorization;
                 using Microsoft.AspNetCore.Cors;
                 using Microsoft.AspNetCore.RateLimiting;
                 using Microsoft.AspNetCore.OutputCaching;
                 using Microsoft.AspNetCore.Http.Timeouts;
                 {{extraUsings}}
                 using System.Threading.Tasks;

                 namespace TestApp;

                 [EndpointGrouping("Products", "/api/products")]
                 {{groupAttributes}}
                 public partial class ProductsGroup;

                 [Endpoint<Get>("", Group = typeof(ProductsGroup))]
                 public partial class GetProductsEndpoint
                 {
                     public async Task<IResult> HandleAsync() => Results.Ok();
                 }
                 """;
    }

    [Fact]
    public void Generate_WhenGroupHasEnableRateLimiting_ThenGroupBuilderRequiresRateLimiting()
    {
        var source = BuildGroupSource("[EnableRateLimiting(\"sliding\")]");

        var generated = GeneratorTestHelper.GetGeneratedSource(source);

        Assert.Contains(".RequireRateLimiting(\"sliding\")", generated);
    }

    [Fact]
    public void Generate_WhenGroupHasDisableRateLimiting_ThenGroupBuilderDisablesRateLimiting()
    {
        var source = BuildGroupSource("[DisableRateLimiting]");

        var generated = GeneratorTestHelper.GetGeneratedSource(source);

        Assert.Contains(".DisableRateLimiting()", generated);
    }

    [Fact]
    public void Generate_WhenGroupHasRequireAntiforgeryToken_ThenGroupBuilderRequiresAntiforgery()
    {
        var source = BuildGroupSource(
            "[RequireAntiforgeryToken]",
            "using Microsoft.AspNetCore.Antiforgery;"
        );

        var generated = GeneratorTestHelper.GetGeneratedSource(source);

        Assert.Contains(".RequireAntiforgery()", generated);
    }

    [Fact]
    public void Generate_WhenGroupHasRequireAntiforgeryTokenFalse_ThenGroupBuilderDisablesAntiforgery()
    {
        var source = BuildGroupSource(
            "[RequireAntiforgeryToken(false)]",
            "using Microsoft.AspNetCore.Antiforgery;"
        );

        var generated = GeneratorTestHelper.GetGeneratedSource(source);

        Assert.Contains(".DisableAntiforgery()", generated);
    }

    [Fact]
    public void Generate_WhenGroupHasDisableHttpMetrics_ThenGroupBuilderDisablesMetrics()
    {
        var source = BuildGroupSource("[DisableHttpMetrics]");

        var generated = GeneratorTestHelper.GetGeneratedSource(source);

        Assert.Contains(".DisableHttpMetrics()", generated);
    }

    [Fact]
    public void Generate_WhenGroupHasOutputCache_ThenGroupBuilderCachesOutput()
    {
        var source = BuildGroupSource("[OutputCache(PolicyName = \"default\")]");

        var generated = GeneratorTestHelper.GetGeneratedSource(source);

        Assert.Contains(".CacheOutput(\"default\")", generated);
    }

    [Fact]
    public void Generate_WhenGroupHasEnableCors_ThenGroupBuilderRequiresCors()
    {
        var source = BuildGroupSource("[EnableCors(\"MyPolicy\")]");

        var generated = GeneratorTestHelper.GetGeneratedSource(source);

        Assert.Contains(".RequireCors(\"MyPolicy\")", generated);
    }

    [Fact]
    public void Generate_WhenGroupHasAllowAnonymous_ThenGroupBuilderAllowsAnonymous()
    {
        var source = BuildGroupSource("[AllowAnonymous]");

        var generated = GeneratorTestHelper.GetGeneratedSource(source);

        Assert.Contains(".AllowAnonymous()", generated);
    }

    [Fact]
    public void Generate_WhenGroupHasAuthorize_ThenGroupBuilderRequiresAuthorization()
    {
        var source = BuildGroupSource("[Authorize]");

        var generated = GeneratorTestHelper.GetGeneratedSource(source);

        Assert.Contains(".RequireAuthorization()", generated);
    }

    [Fact]
    public void Generate_WhenGroupHasAuthorizeWithPolicy_ThenGroupBuilderRequiresAuthorizationWithPolicy()
    {
        var source = BuildGroupSource("[Authorize(\"AdminPolicy\")]");

        var generated = GeneratorTestHelper.GetGeneratedSource(source);

        Assert.Contains(".RequireAuthorization(\"AdminPolicy\")", generated);
    }

    [Fact]
    public void Generate_WhenGroupHasRequestTimeout_ThenGroupBuilderSetsRequestTimeout()
    {
        var source = BuildGroupSource("[RequestTimeout(\"short\")]");

        var generated = GeneratorTestHelper.GetGeneratedSource(source);

        Assert.Contains(".WithRequestTimeout(\"short\")", generated);
    }

    [Fact]
    public void Generate_WhenGroupHasEndpointFilter_ThenGroupBuilderAddsFilter()
    {
        const string source = """
            using Nameless.Web.Http.Endpoints.Attributes;
            using Microsoft.AspNetCore.Http;
            using System.Threading;
            using System.Threading.Tasks;
            using System.ValueTuple;

            namespace TestApp;

            public class LoggingFilter : IEndpointFilter
            {
                public ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext ctx, EndpointFilterDelegate next)
                    => next(ctx);
            }

            [EndpointGrouping("Products", "/api/products")]
            [EndpointFilter<LoggingFilter>]
            public partial class ProductsGroup;

            [Endpoint<Get>("", Group = typeof(ProductsGroup))]
            public partial class GetProductsEndpoint
            {
                public async Task<IResult> HandleAsync() => Results.Ok();
            }
            """;

        var generated = GeneratorTestHelper.GetGeneratedSource(source);

        Assert.Contains(".AddEndpointFilter<global::TestApp.LoggingFilter>()", generated);
    }

    [Fact]
    public void Generate_WhenGroupAndEndpointBothHaveFilters_ThenBothFiltersPresent()
    {
        const string source = """
            using Nameless.Web.Http.Endpoints.Attributes;
            using Microsoft.AspNetCore.Http;
            using System.Threading;
            using System.Threading.Tasks;
            using System.ValueTuple;

            namespace TestApp;

            public class GroupFilter : IEndpointFilter
            {
                public ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext ctx, EndpointFilterDelegate next)
                    => next(ctx);
            }

            public class EndpointSpecificFilter : IEndpointFilter
            {
                public ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext ctx, EndpointFilterDelegate next)
                    => next(ctx);
            }

            [EndpointGrouping("Products", "/api/products")]
            [EndpointFilter<GroupFilter>]
            public partial class ProductsGroup;

            [Endpoint<Get>("", Group = typeof(ProductsGroup))]
            [EndpointFilter<EndpointSpecificFilter>]
            public partial class GetProductsEndpoint
            {
                public async Task<IResult> HandleAsync() => Results.Ok();
            }
            """;

        var generated = GeneratorTestHelper.GetGeneratedSource(source);

        Assert.Contains("AddEndpointFilter<global::TestApp.GroupFilter>()", generated);
        Assert.Contains("AddEndpointFilter<global::TestApp.EndpointSpecificFilter>()", generated);
    }

    [Fact]
    public void Generate_WhenGroupHasDisableRequestTimeout_ThenGroupBuilderDisablesRequestTimeout()
    {
        var source = BuildGroupSource(
            "[DisableRequestTimeout]",
            "using Microsoft.AspNetCore.Http.Timeouts;"
        );

        var generated = GeneratorTestHelper.GetGeneratedSource(source);

        Assert.Contains(".DisableRequestTimeout()", generated);
    }

    [Fact]
    public void Generate_WhenGroupHasMetadata_ThenMetadataAppearsInGroupPartialNotRegistration()
    {
        var source = BuildGroupSource("[Authorize]");

        var generated = GeneratorTestHelper.GetGeneratedSource(source);

        // Group partial (UsersGroup.g.cs) has the RequireAuthorization call
        Assert.Contains(".RequireAuthorization()", generated);
        // Registration file delegates to the Create method
        Assert.Contains("ProductsGroup.Create(", generated);
    }
}
