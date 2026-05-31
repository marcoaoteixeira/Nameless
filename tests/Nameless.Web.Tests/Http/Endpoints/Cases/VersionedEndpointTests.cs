using Microsoft.CodeAnalysis;
using Nameless.Web.Http.Endpoints.Infrastructure;

namespace Nameless.Web.Http.Endpoints.Cases;

public sealed class VersionedEndpointTests
{
    private const string SingleVersionEndpoint = """
        using Nameless.Web.Http.Endpoints.Attributes;
        using Microsoft.AspNetCore.Http;
        using System.Threading.Tasks;

        namespace TestApp;

        [EndpointGrouping("Users", "/api/v{version:apiVersion}/users")]
        public partial class UsersGroup;

        [Endpoint<Get>("/{id}", Group = typeof(UsersGroup))]
        [Deprecate("1.0.0")]
        public partial class GetUserEndpoint
        {
            public async Task<IResult> HandleAsync(int id) => Results.Ok(id);
        }
        """;

    [Fact]
    public void Generate_WhenSingleVersion_ThenEmitsHasApiVersion()
    {
        var generated = GeneratorTestHelper.GetGeneratedSource(SingleVersionEndpoint);

        Assert.Contains("HasApiVersion(new global::Asp.Versioning.ApiVersion(1, 0))", generated);
    }

    [Fact]
    public void Generate_WhenSingleVersion_ThenEmitsMapToApiVersion()
    {
        var generated = GeneratorTestHelper.GetGeneratedSource(SingleVersionEndpoint);

        Assert.Contains("MapToApiVersion(new global::Asp.Versioning.ApiVersion(1, 0))", generated);
    }

    [Fact]
    public void Generate_WhenSingleVersion_ThenEmitsVersionSet()
    {
        var generated = GeneratorTestHelper.GetGeneratedSource(SingleVersionEndpoint);

        Assert.Contains("NewApiVersionSet()", generated);
        Assert.Contains("ReportApiVersions()", generated);
        Assert.Contains(".Build()", generated);
    }

    [Fact]
    public void Generate_WhenMultipleVersionsInGroup_ThenVersionSetHasAllVersions()
    {
        const string source = """
            using Nameless.Web.Http.Endpoints.Attributes;
            using Microsoft.AspNetCore.Http;
            using System.Threading.Tasks;

            namespace TestApp;

            [EndpointGrouping("Users", "/api/v{version:apiVersion}/users")]
            public partial class UsersGroup;

            [Endpoint<Get>("/{id}", Group = typeof(UsersGroup))]
            [Deprecate("1.0.0")]
            public partial class GetUserEndpointV1
            {
                public async Task<IResult> HandleAsync(int id) => Results.Ok(id);
            }

            [Endpoint<Get>("/{id}", Group = typeof(UsersGroup))]
            [Deprecate("2.0.0")]
            public partial class GetUserEndpointV2
            {
                public async Task<IResult> HandleAsync(int id) => Results.Ok(id);
            }
            """;

        var generated = GeneratorTestHelper.GetGeneratedSource(source);

        Assert.Contains("HasApiVersion(new global::Asp.Versioning.ApiVersion(1, 0))", generated);
        Assert.Contains("HasApiVersion(new global::Asp.Versioning.ApiVersion(2, 0))", generated);
    }

    [Fact]
    public void Generate_WhenGroupHasDeclaredVersions_ThenVersionSetUsesGroupVersions()
    {
        const string source = """
            using Nameless.Web.Http.Endpoints.Attributes;
            using Microsoft.AspNetCore.Http;
            using System.Threading.Tasks;

            namespace TestApp;

            [EndpointGrouping("Users", "/api/v{version:apiVersion}/users", Versions = ["1.0", "2.0"])]
            public partial class UsersGroup;

            [Endpoint<Get>("/{id}", Group = typeof(UsersGroup))]
            [Deprecate("1.0")]
            public partial class GetUserEndpointV1
            {
                public async Task<IResult> HandleAsync(int id) => Results.Ok(id);
            }

            [Endpoint<Get>("/{id}", Group = typeof(UsersGroup))]
            [Deprecate("2.0")]
            public partial class GetUserEndpointV2
            {
                public async Task<IResult> HandleAsync(int id) => Results.Ok(id);
            }
            """;

        var generated = GeneratorTestHelper.GetGeneratedSource(source);

        Assert.Contains("HasApiVersion(new global::Asp.Versioning.ApiVersion(1, 0))", generated);
        Assert.Contains("HasApiVersion(new global::Asp.Versioning.ApiVersion(2, 0))", generated);
    }

    [Fact]
    public void Generate_WhenInvalidVersionString_ThenEmitsDiagnosticENDPOINTS003()
    {
        const string source = """
            using Nameless.Web.Http.Endpoints.Attributes;
            using Microsoft.AspNetCore.Http;
            using System.Threading.Tasks;

            namespace TestApp;

            [Endpoint<Get>("/users")]
            [Deprecate("not-a-version")]
            public partial class GetUsersEndpoint
            {
                public async Task<IResult> HandleAsync() => Results.Ok();
            }
            """;

        var diagnostics = GeneratorTestHelper.GetDiagnostics(source);

        Assert.Contains(diagnostics, d => d.Id == "ENDPOINTS003");
    }

    [Fact]
    public void Generate_WhenNoVersionAttributes_ThenNoVersionSetEmitted()
    {
        const string source = """
            using Nameless.Web.Http.Endpoints.Attributes;
            using Microsoft.AspNetCore.Http;
            using System.Threading.Tasks;

            namespace TestApp;

            [EndpointGrouping("Users", "/api/users")]
            public partial class UsersGroup;

            [Endpoint<Get>("/users", Group = typeof(UsersGroup))]
            public partial class GetUsersEndpoint
            {
                public async Task<IResult> HandleAsync() => Results.Ok();
            }
            """;

        var generated = GeneratorTestHelper.GetGeneratedSource(source);

        Assert.DoesNotContain("NewApiVersionSet()", generated);
        Assert.DoesNotContain("WithApiVersionSet(", generated);
    }

    [Fact]
    public void Generate_WhenGroupVersionsContainsInvalidString_ThenEmitsDiagnosticENDPOINTS003()
    {
        const string source = """
            using Nameless.Web.Http.Endpoints.Attributes;
            using Microsoft.AspNetCore.Http;
            using System.Threading.Tasks;

            namespace TestApp;

            [EndpointGrouping("Users", "/api/v{version:apiVersion}/users", Versions = ["bad-version"])]
            public partial class UsersGroup;

            [Endpoint<Get>("/{id}", Group = typeof(UsersGroup))]
            public partial class GetUserEndpoint
            {
                public async Task<IResult> HandleAsync(int id) => Results.Ok(id);
            }
            """;

        var diagnostics = GeneratorTestHelper.GetDiagnostics(source);

        Assert.Contains(diagnostics, d => d.Id == "ENDPOINTS003" && d.Severity == DiagnosticSeverity.Error);
    }

    [Fact]
    public void Generate_WhenGroupDeclaredVersionsExceedEndpointVersions_ThenAllDeclaredVersionsInSet()
    {
        const string source = """
            using Nameless.Web.Http.Endpoints.Attributes;
            using Microsoft.AspNetCore.Http;
            using System.Threading.Tasks;

            namespace TestApp;

            [EndpointGrouping("Users", "/api/v{version:apiVersion}/users", Versions = ["1.0", "2.0", "3.0"])]
            public partial class UsersGroup;

            [Endpoint<Get>("/{id}", Group = typeof(UsersGroup))]
            [Deprecate("1.0")]
            public partial class GetUserEndpointV1
            {
                public async Task<IResult> HandleAsync(int id) => Results.Ok(id);
            }
            """;

        var generated = GeneratorTestHelper.GetGeneratedSource(source);

        Assert.Contains("HasApiVersion(new global::Asp.Versioning.ApiVersion(1, 0))", generated);
        Assert.Contains("HasApiVersion(new global::Asp.Versioning.ApiVersion(2, 0))", generated);
        Assert.Contains("HasApiVersion(new global::Asp.Versioning.ApiVersion(3, 0))", generated);
    }

    [Fact]
    public void Generate_WhenDeprecatedVersion_ThenEmitsHasDeprecatedApiVersion()
    {
        const string source = """
            using Nameless.Web.Http.Endpoints.Attributes;
            using Microsoft.AspNetCore.Http;
            using System.Threading.Tasks;

            namespace TestApp;

            [EndpointGrouping("Legacy", "/api/legacy/users")]
            public partial class LegacyGroup;

            [Endpoint<Get>("/users", Group = typeof(LegacyGroup))]
            [Deprecate("1.0.0", Deprecated = true)]
            public partial class GetUsersV1Endpoint
            {
                public async Task<IResult> HandleAsync() => Results.Ok();
            }
            """;

        var generated = GeneratorTestHelper.GetGeneratedSource(source);

        Assert.Contains("HasDeprecatedApiVersion(new global::Asp.Versioning.ApiVersion(1, 0))", generated);
    }

    [Fact]
    public void Generate_WhenSingleVersionedUngroupedEndpoint_ThenEmitsVersionSet()
    {
        const string source = """
            using Nameless.Web.Http.Endpoints.Attributes;
            using Microsoft.AspNetCore.Http;
            using System.Threading.Tasks;

            namespace TestApp;

            [Endpoint<Get>("/hello-world")]
            [Deprecate("1.0")]
            public partial class HelloWorldEndpoint
            {
                public async Task<IResult> HandleAsync() => Results.Ok();
            }
            """;

        var generated = GeneratorTestHelper.GetGeneratedSource(source);

        Assert.Contains("NewApiVersionSet()", generated);
        Assert.Contains("HasApiVersion(new global::Asp.Versioning.ApiVersion(1, 0))", generated);
        Assert.Contains("WithApiVersionSet(", generated);
        Assert.Contains("MapToApiVersion(new global::Asp.Versioning.ApiVersion(1, 0))", generated);
    }

    [Fact]
    public void Generate_WhenTwoVersionedUngroupedEndpointsSameRoute_ThenShareSingleVersionSet()
    {
        const string source = """
            using Nameless.Web.Http.Endpoints.Attributes;
            using Microsoft.AspNetCore.Http;
            using System.Threading.Tasks;

            namespace TestApp;

            [Endpoint<Get>("/hello-world")]
            [Deprecate("1.0")]
            public partial class HelloWorldV1Endpoint
            {
                public async Task<IResult> HandleAsync() => Results.Ok("v1");
            }

            [Endpoint<Get>("/hello-world")]
            [Deprecate("2.0")]
            public partial class HelloWorldV2Endpoint
            {
                public async Task<IResult> HandleAsync() => Results.Ok("v2");
            }
            """;

        var generated = GeneratorTestHelper.GetGeneratedSource(source);

        Assert.Equal(1, generated.Split("NewApiVersionSet()").Length - 1);
        Assert.Contains("HasApiVersion(new global::Asp.Versioning.ApiVersion(1, 0))", generated);
        Assert.Contains("HasApiVersion(new global::Asp.Versioning.ApiVersion(2, 0))", generated);
        Assert.Contains("MapToApiVersion(new global::Asp.Versioning.ApiVersion(1, 0))", generated);
        Assert.Contains("MapToApiVersion(new global::Asp.Versioning.ApiVersion(2, 0))", generated);
    }

    [Fact]
    public void Generate_WhenVersionedUngroupedEndpointsDifferentRoutes_ThenSeparateVersionSets()
    {
        const string source = """
            using Nameless.Web.Http.Endpoints.Attributes;
            using Microsoft.AspNetCore.Http;
            using System.Threading.Tasks;

            namespace TestApp;

            [Endpoint<Get>("/foo")]
            [Deprecate("1.0")]
            public partial class FooEndpoint
            {
                public async Task<IResult> HandleAsync() => Results.Ok("foo");
            }

            [Endpoint<Get>("/bar")]
            [Deprecate("2.0")]
            public partial class BarEndpoint
            {
                public async Task<IResult> HandleAsync() => Results.Ok("bar");
            }
            """;

        var generated = GeneratorTestHelper.GetGeneratedSource(source);

        Assert.Equal(2, generated.Split("NewApiVersionSet()").Length - 1);
    }

    [Fact]
    public void Generate_WhenUngroupedEndpointHasNoVersion_ThenNoVersionSetEmitted()
    {
        const string source = """
            using Nameless.Web.Http.Endpoints.Attributes;
            using Microsoft.AspNetCore.Http;
            using System.Threading.Tasks;

            namespace TestApp;

            [Endpoint<Get>("/health")]
            public partial class HealthEndpoint
            {
                public async Task<IResult> HandleAsync() => Results.Ok("Healthy");
            }
            """;

        var generated = GeneratorTestHelper.GetGeneratedSource(source);

        Assert.DoesNotContain("NewApiVersionSet()", generated);
        Assert.DoesNotContain("WithApiVersionSet(", generated);
        Assert.DoesNotContain("MapGroup(", generated);
    }
}
