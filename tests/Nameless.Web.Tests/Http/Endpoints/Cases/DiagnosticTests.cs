using Microsoft.CodeAnalysis;
using Nameless.Web.Http.Endpoints.Infrastructure;

namespace Nameless.Web.Http.Endpoints.Cases;

public sealed class DiagnosticTests
{
    [Fact]
    public void Generate_WhenGroupTypeHasNoGroupAttribute_ThenEmitsENDPOINTS007()
    {
        const string source = """
            using Nameless.Web.Http.Endpoints.Attributes;
            using Microsoft.AspNetCore.Http;
            using System.Threading.Tasks;

            namespace TestApp;

            public class NotAGroup;

            [Endpoint<Get>("/a", Group = typeof(NotAGroup))]
            public class GetItemV1Endpoint
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

            [Endpoint<Get>("/items")]
            public class GetItemsEndpoint { }
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

            [Endpoint<Get>("/items")]
            [Version("bad-version")]
            public class GetItemsEndpoint
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

            [Endpoint<Get>("/items")]
            public class GetItemsEndpoint
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

            [Endpoint<Get>("/items")]
            [Authorize]
            [AllowAnonymous]
            public class GetItemsEndpoint
            {
                public async Task<IResult> HandleAsync() => Results.Ok();
            }
            """;

        var diagnostics = GeneratorTestHelper.GetDiagnostics(source);

        Assert.Contains(diagnostics, d => d.Id == "ENDPOINTS006" && d.Severity == DiagnosticSeverity.Info);
    }
}
