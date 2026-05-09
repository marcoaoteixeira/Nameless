using Microsoft.CodeAnalysis;
using Nameless.Web.Http.Endpoints.Infrastructure;

namespace Nameless.Web.Http.Endpoints.Cases;

public sealed class GroupedEndpointTests
{
    private const string TwoEndpointsSameGroup = """
        using Nameless.Web.Http.Endpoints.Attributes;
        using Microsoft.AspNetCore.Http;
        using System.Threading.Tasks;

        namespace TestApp;

        [Group("Users", "/api/users")]
        public class UsersGroup;

        [Endpoint<Get>("/{id}", Group = typeof(UsersGroup))]
        public class GetUserEndpoint
        {
            public async Task<IResult> HandleAsync(int id) => Results.Ok(id);
        }

        [Endpoint<Post>("", Group = typeof(UsersGroup))]
        public class CreateUserEndpoint
        {
            public async Task<IResult> HandleAsync() => Results.Ok();
        }
        """;

    [Fact]
    public void Generate_WhenTwoEndpointsSameGroup_ThenSingleMapGroupEmitted()
    {
        var generated = GeneratorTestHelper.GetGeneratedSource(TwoEndpointsSameGroup);

        var groupCount = CountOccurrences(generated, "MapGroup(\"/api/users\")");
        Assert.Equal(1, groupCount);
    }

    [Fact]
    public void Generate_WhenTwoEndpointsSameGroup_ThenBothEndpointsRegistered()
    {
        var generated = GeneratorTestHelper.GetGeneratedSource(TwoEndpointsSameGroup);

        Assert.Contains("global::TestApp.GetUserEndpoint", generated);
        Assert.Contains("global::TestApp.CreateUserEndpoint", generated);
    }

    [Fact]
    public void Generate_WhenGroupedEndpoint_ThenWithGroupNameNotEmitted()
    {
        var generated = GeneratorTestHelper.GetGeneratedSource(TwoEndpointsSameGroup);

        Assert.DoesNotContain("WithGroupName", generated);
    }

    [Fact]
    public void Generate_WhenGroupTypeHasNoGroupAttribute_ThenEmitsDiagnosticENDPOINTS007()
    {
        const string source = """
            using Nameless.Web.Http.Endpoints.Attributes;
            using Microsoft.AspNetCore.Http;
            using System.Threading.Tasks;

            namespace TestApp;

            public class NotAGroup;

            [Endpoint<Get>("/{id}", Group = typeof(NotAGroup))]
            public class GetUserEndpoint
            {
                public async Task<IResult> HandleAsync(int id) => Results.Ok(id);
            }
            """;

        var diagnostics = GeneratorTestHelper.GetDiagnostics(source);

        Assert.Contains(diagnostics, d => d.Id == "ENDPOINTS007" && d.Severity == DiagnosticSeverity.Error);
    }

    [Fact]
    public void Generate_WhenGroupTypeHasNoGroupAttribute_ThenGroupNotEmitted()
    {
        const string source = """
            using Nameless.Web.Http.Endpoints.Attributes;
            using Microsoft.AspNetCore.Http;
            using System.Threading.Tasks;

            namespace TestApp;

            public class NotAGroup;

            [Endpoint<Get>("/{id}", Group = typeof(NotAGroup))]
            public class GetUserEndpoint
            {
                public async Task<IResult> HandleAsync(int id) => Results.Ok(id);
            }
            """;

        var generated = GeneratorTestHelper.GetGeneratedSource(source);

        Assert.DoesNotContain("MapGroup", generated);
    }

    [Fact]
    public void Generate_WhenEndpointVersionNotInGroupVersionSet_ThenEmitsDiagnosticENDPOINTS008()
    {
        const string source = """
            using Nameless.Web.Http.Endpoints.Attributes;
            using Microsoft.AspNetCore.Http;
            using System.Threading.Tasks;

            namespace TestApp;

            [Group("Users", "/api/v{version:apiVersion}/users", Versions = ["1.0", "2.0"])]
            public class UsersGroup;

            [Endpoint<Get>("/{id}", Group = typeof(UsersGroup))]
            [Version("3.0")]
            public class GetUserEndpoint
            {
                public async Task<IResult> HandleAsync(int id) => Results.Ok(id);
            }
            """;

        var diagnostics = GeneratorTestHelper.GetDiagnostics(source);

        Assert.Contains(diagnostics, d => d.Id == "ENDPOINTS008" && d.Severity == DiagnosticSeverity.Error);
    }

    [Fact]
    public void Generate_WhenEndpointVersionNotInGroupVersionSet_ThenGroupIsStillEmitted()
    {
        const string source = """
            using Nameless.Web.Http.Endpoints.Attributes;
            using Microsoft.AspNetCore.Http;
            using System.Threading.Tasks;

            namespace TestApp;

            [Group("Users", "/api/v{version:apiVersion}/users", Versions = ["1.0", "2.0"])]
            public class UsersGroup;

            [Endpoint<Get>("/{id}", Group = typeof(UsersGroup))]
            [Version("3.0")]
            public class GetUserEndpoint
            {
                public async Task<IResult> HandleAsync(int id) => Results.Ok(id);
            }
            """;

        var generated = GeneratorTestHelper.GetGeneratedSource(source);

        Assert.Contains("MapGroup(\"/api/v{version:apiVersion}/users\")", generated);
    }

    [Fact]
    public void Generate_WhenGroupMarkerExistsButHasNoEndpoints_ThenNoMapGroupEmitted()
    {
        const string source = """
            using Nameless.Web.Http.Endpoints.Attributes;
            using Microsoft.AspNetCore.Http;
            using System.Threading.Tasks;

            namespace TestApp;

            [Group("Users", "/api/users")]
            public class UsersGroup;

            [Endpoint<Get>("/items")]
            public class GetItemsEndpoint
            {
                public async Task<IResult> HandleAsync() => Results.Ok();
            }
            """;

        var generated = GeneratorTestHelper.GetGeneratedSource(source);

        Assert.DoesNotContain("MapGroup(\"/api/users\")", generated);
    }

    [Fact]
    public void Generate_WhenGroupMarkerHasEmptyName_ThenEmitsDiagnosticENDPOINTS009()
    {
        const string source = """
            using Nameless.Web.Http.Endpoints.Attributes;
            using Microsoft.AspNetCore.Http;
            using System.Threading.Tasks;

            namespace TestApp;

            [Group("", "/api/users")]
            public class UsersGroup;

            [Endpoint<Get>("/{id}", Group = typeof(UsersGroup))]
            public class GetUserEndpoint
            {
                public async Task<IResult> HandleAsync(int id) => Results.Ok(id);
            }
            """;

        var diagnostics = GeneratorTestHelper.GetDiagnostics(source);

        Assert.Contains(diagnostics, d => d.Id == "ENDPOINTS009" && d.Severity == DiagnosticSeverity.Error);
    }

    private static int CountOccurrences(string text, string pattern)
    {
        var count = 0;
        var index = 0;
        while ((index = text.IndexOf(pattern, index, StringComparison.Ordinal)) >= 0)
        {
            count++;
            index += pattern.Length;
        }
        return count;
    }
}
