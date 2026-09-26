using Microsoft.CodeAnalysis;
using Nameless.Web.Http.Endpoints.Infrastructure;

namespace Nameless.Web.Http.Endpoints.Cases;

public sealed class DiagnosticTests
{
    [Fact]
    public void Generate_WhenGroupTypeHasNoEndpointGroupingAttribute_ThenEmitsENDPOINTS007()
    {
        const string source = """
            using Nameless.Web.Http.Endpoints.Attributes;
            using Microsoft.AspNetCore.Http;
            using System.Threading.Tasks;

            namespace TestApp;

            public partial class NotAGroup;

            [Endpoint("/a", Group = typeof(NotAGroup))]
            public partial class GetItemV1Endpoint
            {
                public async Task<IResult> HandleAsync() => Results.Ok();
            }
            """;

        var diagnostics = GeneratorTestHelper.GetDiagnostics(source);

        Assert.Contains(diagnostics, d => d.Id == "ENDPOINTS007" && d.Severity == DiagnosticSeverity.Error);
    }

    [Fact]
    public void Generate_WhenMissingHandleAsync_ThenEmitsENDPOINTS002()
    {
        const string source = """
            using Nameless.Web.Http.Endpoints.Attributes;

            namespace TestApp;

            [Endpoint("/items")]
            public partial class GetItemsEndpoint { }
            """;

        var diagnostics = GeneratorTestHelper.GetDiagnostics(source);

        Assert.Contains(diagnostics, d => d.Id == "ENDPOINTS002" && d.Severity == DiagnosticSeverity.Error);
    }

    [Fact]
    public void Generate_WhenInvalidVersionString_ThenEmitsENDPOINTS003()
    {
        const string source = """
            using Nameless.Web.Http.Endpoints.Attributes;
            using Microsoft.AspNetCore.Http;
            using System.Threading.Tasks;

            namespace TestApp;

            [Endpoint("/items")]
            [Deprecate("bad-version")]
            public partial class GetItemsEndpoint
            {
                public async Task<IResult> HandleAsync() => Results.Ok();
            }
            """;

        var diagnostics = GeneratorTestHelper.GetDiagnostics(source);

        Assert.Contains(diagnostics, d => d.Id == "ENDPOINTS003" && d.Severity == DiagnosticSeverity.Error);
    }

    [Fact]
    public void Generate_WhenHandleAsyncIsPrivate_ThenEmitsENDPOINTS005()
    {
        const string source = """
            using Nameless.Web.Http.Endpoints.Attributes;
            using Microsoft.AspNetCore.Http;
            using System.Threading.Tasks;

            namespace TestApp;

            [Endpoint("/items")]
            public partial class GetItemsEndpoint
            {
                private async Task<IResult> HandleAsync() => Results.Ok();
            }
            """;

        var diagnostics = GeneratorTestHelper.GetDiagnostics(source);

        Assert.Contains(diagnostics, d => d.Id == "ENDPOINTS005" && d.Severity == DiagnosticSeverity.Warning);
    }

    [Fact]
    public void Generate_WhenAuthorizeAndAllowAnonymousBothPresent_ThenEmitsENDPOINTS006()
    {
        const string source = """
            using Nameless.Web.Http.Endpoints.Attributes;
            using Microsoft.AspNetCore.Http;
            using Microsoft.AspNetCore.Authorization;
            using System.Threading.Tasks;

            namespace TestApp;

            [Endpoint("/items")]
            [Authorize]
            [AllowAnonymous]
            public partial class GetItemsEndpoint
            {
                public async Task<IResult> HandleAsync() => Results.Ok();
            }
            """;

        var diagnostics = GeneratorTestHelper.GetDiagnostics(source);

        Assert.Contains(diagnostics, d => d.Id == "ENDPOINTS006" && d.Severity == DiagnosticSeverity.Info);
    }

    [Fact]
    public void Generate_WhenEndpointClassIsNotPartial_ThenEmitsENDPOINTS010()
    {
        const string source = """
            using Nameless.Web.Http.Endpoints.Attributes;
            using Microsoft.AspNetCore.Http;
            using System.Threading.Tasks;

            namespace TestApp;

            [Endpoint("/items")]
            public class GetItemsEndpoint
            {
                public async Task<IResult> HandleAsync() => Results.Ok();
            }
            """;

        var diagnostics = GeneratorTestHelper.GetDiagnostics(source);

        Assert.Contains(diagnostics, d => d.Id == "ENDPOINTS010" && d.Severity == DiagnosticSeverity.Error);
    }

    [Fact]
    public void Generate_WhenGroupClassIsNotPartial_ThenEmitsENDPOINTS010()
    {
        const string source = """
            using Nameless.Web.Http.Endpoints.Attributes;
            using Microsoft.AspNetCore.Http;
            using System.Threading.Tasks;

            namespace TestApp;

            [EndpointGrouping("Users", "/api/users")]
            public class UsersGroup;

            [Endpoint("/{id}", Group = typeof(UsersGroup))]
            public partial class GetUserEndpoint
            {
                public async Task<IResult> HandleAsync(int id) => Results.Ok(id);
            }
            """;

        var diagnostics = GeneratorTestHelper.GetDiagnostics(source);

        Assert.Contains(diagnostics, d => d.Id == "ENDPOINTS010" && d.Severity == DiagnosticSeverity.Error);
    }

    [Fact]
    public void Generate_WhenEndpointClassIsNotPartial_ThenNoCodeGenerated()
    {
        const string source = """
            using Nameless.Web.Http.Endpoints.Attributes;
            using Microsoft.AspNetCore.Http;
            using System.Threading.Tasks;

            namespace TestApp;

            [Endpoint("/items")]
            public class GetItemsEndpoint
            {
                public async Task<IResult> HandleAsync() => Results.Ok();
            }
            """;

        var generated = GeneratorTestHelper.GetGeneratedSource(source);

        Assert.DoesNotContain("GetItemsEndpoint", generated);
    }

    [Fact]
    public void Generate_WhenHandleAsyncIsPrivate_ThenCodeIsStillGenerated()
    {
        const string source = """
            using Nameless.Web.Http.Endpoints.Attributes;
            using Microsoft.AspNetCore.Http;
            using System.Threading.Tasks;

            namespace TestApp;

            [Endpoint("/items")]
            public partial class GetItemsEndpoint
            {
                private async Task<IResult> HandleAsync() => Results.Ok();
            }
            """;

        var generated = GeneratorTestHelper.GetGeneratedSource(source);

        Assert.Contains("GetItemsEndpoint", generated);
    }

    [Fact]
    public void Generate_WhenGroupHasAuthorizeAndAllowAnonymous_ThenEmitsENDPOINTS006()
    {
        const string source = """
            using Nameless.Web.Http.Endpoints.Attributes;
            using Microsoft.AspNetCore.Http;
            using Microsoft.AspNetCore.Authorization;
            using System.Threading.Tasks;

            namespace TestApp;

            [EndpointGrouping("Users", "/api/users")]
            [Authorize]
            [AllowAnonymous]
            public partial class UsersGroup;

            [Endpoint("/{id}", Group = typeof(UsersGroup))]
            public partial class GetUserEndpoint
            {
                public async Task<IResult> HandleAsync(int id) => Results.Ok(id);
            }
            """;

        var diagnostics = GeneratorTestHelper.GetDiagnostics(source);

        Assert.Contains(diagnostics, d => d.Id == "ENDPOINTS006" && d.Severity == DiagnosticSeverity.Info);
    }
}
