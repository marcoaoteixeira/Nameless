using Nameless.Web.Http.Endpoints.Infrastructure;

namespace Nameless.Web.Http.Endpoints.Cases;

public sealed class SimpleGetEndpointTests
{
    private const string MinimalGetEndpoint = """
        using Nameless.Web.Http.Endpoints.Attributes;
        using Microsoft.AspNetCore.Http;
        using System.Threading.Tasks;

        namespace TestApp;

        [Endpoint<Get>("/users")]
        public partial class GetUsersEndpoint
        {
            public async Task<IResult> HandleAsync()
                => Results.Ok();
        }
        """;

    [Fact]
    public void Generate_WhenMinimalGetEndpoint_ThenEmitsMapGet()
    {
        var source = GeneratorTestHelper.GetGeneratedSource(MinimalGetEndpoint);

        Assert.Contains("MapGet", source);
        Assert.Contains("\"/users\"", source);
        Assert.Contains("global::TestApp.GetUsersEndpoint", source);
    }

    [Fact]
    public void Generate_WhenMinimalGetEndpoint_ThenEmitsRegisterMethod()
    {
        var source = GeneratorTestHelper.GetGeneratedSource(MinimalGetEndpoint);

        Assert.Contains("RegisterAutoEndpoints", source);
        Assert.Contains("TryAddTransient<global::TestApp.GetUsersEndpoint>", source);
    }

    [Fact]
    public void Generate_WhenMinimalGetEndpoint_ThenEmitsMapMethod()
    {
        var source = GeneratorTestHelper.GetGeneratedSource(MinimalGetEndpoint);

        Assert.Contains("MapAutoEndpoints", source);
    }

    [Fact]
    public void Generate_WhenMinimalGetEndpoint_ThenEndpointResolvesViaFromServices()
    {
        var source = GeneratorTestHelper.GetGeneratedSource(MinimalGetEndpoint);

        Assert.Contains("[global::Microsoft.AspNetCore.Mvc.FromServices] global::TestApp.GetUsersEndpoint", source);
    }

    [Fact]
    public void Generate_WhenMinimalGetEndpoint_ThenPartialClassEmitted()
    {
        var source = GeneratorTestHelper.GetGeneratedSource(MinimalGetEndpoint);

        Assert.Contains("partial class GetUsersEndpoint", source);
        Assert.Contains("internal static void Register(", source);
        Assert.Contains("internal static void Map(", source);
    }

    [Fact]
    public void Generate_WhenMinimalGetEndpoint_ThenRegistrationDelegates()
    {
        var source = GeneratorTestHelper.GetGeneratedSource(MinimalGetEndpoint);

        Assert.Contains("global::TestApp.GetUsersEndpoint.Register(", source);
        Assert.Contains("global::TestApp.GetUsersEndpoint.Map(", source);
    }

    [Fact]
    public void Generate_WhenEndpointHasCancellationToken_ThenLambdaIncludesCancellationTokenParam()
    {
        const string source = """
            using Nameless.Web.Http.Endpoints.Attributes;
            using Microsoft.AspNetCore.Http;
            using System.Threading;
            using System.Threading.Tasks;

            namespace TestApp;

            [Endpoint<Get>("/users")]
            public partial class GetUsersEndpoint
            {
                public async Task<IResult> HandleAsync(CancellationToken ct)
                    => Results.Ok();
            }
            """;

        var generated = GeneratorTestHelper.GetGeneratedSource(source);

        Assert.Contains("global::System.Threading.CancellationToken ct", generated);
    }

    [Fact]
    public void Generate_WhenEndpointHasRouteParam_ThenLambdaIncludesRouteParam()
    {
        const string source = """
            using Nameless.Web.Http.Endpoints.Attributes;
            using Microsoft.AspNetCore.Http;
            using System.Threading.Tasks;

            namespace TestApp;

            [Endpoint<Get>("/users/{id}")]
            public partial class GetUserEndpoint
            {
                public async Task<IResult> HandleAsync(int id)
                    => Results.Ok(id);
            }
            """;

        var generated = GeneratorTestHelper.GetGeneratedSource(source);

        Assert.Contains("int id", generated);
        Assert.Contains("HandleAsync(id)", generated);
    }

    [Fact]
    public void Generate_WhenNoHandleAsyncMethod_ThenEmitsDiagnosticENDPOINTS002()
    {
        const string source = """
            using Nameless.Web.Http.Endpoints.Attributes;

            namespace TestApp;

            [Endpoint<Get>("/users")]
            public partial class GetUsersEndpoint { }
            """;

        var diagnostics = GeneratorTestHelper.GetDiagnostics(source);

        Assert.Contains(diagnostics, d => d.Id == "ENDPOINTS002");
    }

    [Fact]
    public void Generate_WhenHandleAsyncIsPrivate_ThenMapGetStillEmitted()
    {
        // ENDPOINTS005 is a warning, not a blocker — generation proceeds so the
        // compiler produces the actionable "inaccessible member" error at the call site.
        const string source = """
            using Nameless.Web.Http.Endpoints.Attributes;
            using Microsoft.AspNetCore.Http;
            using System.Threading.Tasks;

            namespace TestApp;

            [Endpoint<Get>("/users")]
            public partial class GetUsersEndpoint
            {
                private async Task<IResult> HandleAsync() => Results.Ok();
            }
            """;

        var generated = GeneratorTestHelper.GetGeneratedSource(source);

        Assert.Contains("MapGet", generated);
    }

    [Fact]
    public void Generate_WhenRouteTemplateContainsQuote_ThenGeneratedCodeCompiles()
    {
        const string source = """
            using Nameless.Web.Http.Endpoints.Attributes;
            using Microsoft.AspNetCore.Http;
            using System.Threading.Tasks;

            namespace TestApp;

            [Endpoint<Get>("/users/\"special\"")]
            public partial class GetUsersEndpoint
            {
                public async Task<IResult> HandleAsync() => Results.Ok();
            }
            """;

        var generated = GeneratorTestHelper.GetGeneratedSource(source);

        Assert.Contains("/users/\\\"special\\\"", generated);
    }

    [Fact]
    public void Generate_WhenEndpointHasNoGroup_ThenMappedOnRootBuilder()
    {
        var generated = GeneratorTestHelper.GetGeneratedSource(MinimalGetEndpoint);

        Assert.DoesNotContain("MapGroup", generated);
        // Registration delegates to the endpoint's static Map, passing self (root builder).
        Assert.Contains("GetUsersEndpoint.Map(self)", generated);
    }

    [Fact]
    public void Generate_WhenGeneratedSource_ThenContainsAutoGeneratedComment()
    {
        var generated = GeneratorTestHelper.GetGeneratedSource(MinimalGetEndpoint);

        Assert.Contains("// <auto-generated/>", generated);
    }

    [Fact]
    public void Generate_WhenGeneratedSource_ThenNullableIsEnabled()
    {
        var generated = GeneratorTestHelper.GetGeneratedSource(MinimalGetEndpoint);

        Assert.Contains("#nullable enable", generated);
    }

    [Fact]
    public void Generate_WhenEndpointHasName_ThenEmitsWithName()
    {
        const string source = """
            using Nameless.Web.Http.Endpoints.Attributes;
            using Microsoft.AspNetCore.Http;
            using System.Threading.Tasks;

            namespace TestApp;

            [Endpoint<Get>("/users", Name = "ListUsers")]
            public partial class GetUsersEndpoint
            {
                public async Task<IResult> HandleAsync() => Results.Ok();
            }
            """;

        var generated = GeneratorTestHelper.GetGeneratedSource(source);

        Assert.Contains(".WithName(\"ListUsers\")", generated);
    }

    [Fact]
    public void Generate_WhenEndpointHasNoName_ThenWithNameUsesClassName()
    {
        var generated = GeneratorTestHelper.GetGeneratedSource(MinimalGetEndpoint);

        Assert.Contains(".WithName(\"GetUsersEndpoint\")", generated);
    }

    [Fact]
    public void Generate_WhenEndpointHasTags_ThenEmitsWithTags()
    {
        const string source = """
            using Nameless.Web.Http.Endpoints.Attributes;
            using Microsoft.AspNetCore.Http;
            using System.Threading.Tasks;

            namespace TestApp;

            [Endpoint<Get>("/users", Tags = ["Users", "Api"])]
            public partial class GetUsersEndpoint
            {
                public async Task<IResult> HandleAsync() => Results.Ok();
            }
            """;

        var generated = GeneratorTestHelper.GetGeneratedSource(source);

        Assert.Contains(".WithTags(\"Users\", \"Api\")", generated);
    }
}
