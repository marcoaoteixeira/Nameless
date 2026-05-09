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
        public class GetUsersEndpoint
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
    public void Generate_WhenMinimalGetEndpoint_ThenEmitsAddEndpoints()
    {
        var source = GeneratorTestHelper.GetGeneratedSource(MinimalGetEndpoint);

        Assert.Contains("RegisterAutoEndpoints", source);
        Assert.Contains("TryAddTransient<global::TestApp.GetUsersEndpoint>", source);
    }

    [Fact]
    public void Generate_WhenMinimalGetEndpoint_ThenEmitsMapEndpoints()
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
    public void Generate_WhenEndpointHasCancellationToken_ThenLambdaIncludesCancellationTokenParam()
    {
        const string source = """
            using Nameless.Web.Http.Endpoints.Attributes;
            using Microsoft.AspNetCore.Http;
            using System.Threading;
            using System.Threading.Tasks;

            namespace TestApp;

            [Endpoint<Get>("/users")]
            public class GetUsersEndpoint
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
            public class GetUserEndpoint
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
            public class GetUsersEndpoint { }
            """;

        var diagnostics = GeneratorTestHelper.GetDiagnostics(source);

        Assert.Contains(diagnostics, d => d.Id == "ENDPOINTS002");
    }

    [Fact]
    public void Generate_WhenHandleAsyncIsPrivate_ThenNoMapGetEmitted()
    {
        const string source = """
            using Nameless.Web.Http.Endpoints.Attributes;
            using Microsoft.AspNetCore.Http;
            using System.Threading.Tasks;

            namespace TestApp;

            [Endpoint<Get>("/users")]
            public class GetUsersEndpoint
            {
                private async Task<IResult> HandleAsync() => Results.Ok();
            }
            """;

        var generated = GeneratorTestHelper.GetGeneratedSource(source);

        Assert.DoesNotContain("MapGet", generated);
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
            public class GetUsersEndpoint
            {
                public async Task<IResult> HandleAsync() => Results.Ok();
            }
            """;

        var generated = GeneratorTestHelper.GetGeneratedSource(source);

        // generated code must contain: "/users/\"special\""
        Assert.Contains("/users/\\\"special\\\"", generated);
    }

    [Fact]
    public void Generate_WhenEndpointHasNoGroup_ThenRegisteredOnRootBuilder()
    {
        var generated = GeneratorTestHelper.GetGeneratedSource(MinimalGetEndpoint);

        Assert.DoesNotContain("MapGroup", generated);
        Assert.Contains("self.MapGet", generated);
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
            public class GetUsersEndpoint
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
            public class GetUsersEndpoint
            {
                public async Task<IResult> HandleAsync() => Results.Ok();
            }
            """;

        var generated = GeneratorTestHelper.GetGeneratedSource(source);

        Assert.Contains(".WithTags(\"Users\", \"Api\")", generated);
    }
}
