using Nameless.Testing.Tools.Attributes;
using Nameless.Web.Http.Endpoints.Infrastructure;

namespace Nameless.Web.Http.Endpoints.UseCases.Attributes.Authorization;

[UnitTest]
public class WithUseAuthorizationConvention {
    private static string UseAuthorizationWithNoArgumentsEndpoint => SourceCodeHelper.Write(
        """
        [Endpoint<Get>]
        [UseAuthorization()]
        public partial class UseAuthorizationWithNoArgumentsEndpoint {
            public Task<IResult> HandleAsync() {
                return Task.FromResult<IResult>(TypedResults.Ok("OK"));
            }
        }
        """
    );

    private static string UseAuthorizationWithPolicyNameEndpoint => SourceCodeHelper.Write(
        """
        [Endpoint<Get>]
        [UseAuthorization(PolicyName = "auth-policy")]
        public partial class UseAuthorizationWithPolicyNameEndpoint {
            public Task<IResult> HandleAsync() {
                return Task.FromResult<IResult>(TypedResults.Ok("OK"));
            }
        }
        """
    );

    private static string UseAuthorizationWithPolicyNameRoleEndpoint => SourceCodeHelper.Write(
        """
        [Endpoint<Get>]
        [UseAuthorization(PolicyName = "auth-policy", Roles = "Admin, User")]
        public partial class UseAuthorizationWithPolicyNameRoleEndpoint {
            public Task<IResult> HandleAsync() {
                return Task.FromResult<IResult>(TypedResults.Ok("OK"));
            }
        }
        """
    );

    private static string UseAuthorizationWithPolicyNameRoleAuthenticationSchemesEndpoint => SourceCodeHelper.Write(
        """
        [Endpoint<Get>]
        [UseAuthorization(PolicyName = "auth-policy", Roles = "Admin, User", AuthenticationSchemes = "Bearer, Cookie")]
        public partial class UseAuthorizationWithPolicyNameRoleAuthenticationSchemesEndpoint {
            public Task<IResult> HandleAsync() {
                return Task.FromResult<IResult>(TypedResults.Ok("OK"));
            }
        }
        """
    );

    private static string UseAuthorizationWithPolicyNameAuthenticationSchemesEndpoint => SourceCodeHelper.Write(
        """
        [Endpoint<Get>]
        [UseAuthorization(PolicyName = "auth-policy", AuthenticationSchemes = "Bearer, Cookie")]
        public partial class UseAuthorizationWithPolicyNameAuthenticationSchemesEndpoint {
            public Task<IResult> HandleAsync() {
                return Task.FromResult<IResult>(TypedResults.Ok("OK"));
            }
        }
        """
    );

    [Fact]
    public void WhenEndpointClassMarkUseAuthorizationWithNoArguments_ThenEmitConvention() {
        var source = GeneratorTestHelper.GetGeneratedSource(UseAuthorizationWithNoArgumentsEndpoint);

        Assert.Contains($"{SourceCodeHelper.Namespace}.UseAuthorizationWithNoArgumentsEndpoint", source);
        Assert.Matches(@"UseAuthorizationWithNoArgumentsEndpoint\.Register\(.*\);", source);
        Assert.Matches(@"UseAuthorizationWithNoArgumentsEndpoint\.Map\(.*\);", source);
        Assert.Matches(@"\.RequireAuthorization\(\)", source);
    }

    [Fact]
    public void WhenEndpointClassMarkUseAuthorizationWithPolicyName_ThenEmitConvention() {
        var source = GeneratorTestHelper.GetGeneratedSource(UseAuthorizationWithPolicyNameEndpoint);

        Assert.Contains($"{SourceCodeHelper.Namespace}.UseAuthorizationWithPolicyNameEndpoint", source);
        Assert.Matches(@"UseAuthorizationWithPolicyNameEndpoint\.Register\(.*\);", source);
        Assert.Matches(@"UseAuthorizationWithPolicyNameEndpoint\.Map\(.*\);", source);
        Assert.Matches("""\.RequireAuthorization\(new AuthorizeAttribute { Policy = "auth-policy" }\)""", source);
    }

    [Fact]
    public void WhenEndpointClassMarkUseAuthorizationWithPolicyNameRole_ThenEmitConvention() {
        var source = GeneratorTestHelper.GetGeneratedSource(UseAuthorizationWithPolicyNameRoleEndpoint);

        Assert.Contains($"{SourceCodeHelper.Namespace}.UseAuthorizationWithPolicyNameRoleEndpoint", source);
        Assert.Matches(@"UseAuthorizationWithPolicyNameRoleEndpoint\.Register\(.*\);", source);
        Assert.Matches(@"UseAuthorizationWithPolicyNameRoleEndpoint\.Map\(.*\);", source);
        Assert.Matches("""\.RequireAuthorization\(new AuthorizeAttribute { Policy = "auth-policy", Roles = "Admin, User" }\)""", source);
    }

    [Fact]
    public void WhenEndpointClassMarkUseAuthorizationWithPolicyNameRoleAuthenticationSchemes_ThenEmitConvention() {
        var source = GeneratorTestHelper.GetGeneratedSource(UseAuthorizationWithPolicyNameRoleAuthenticationSchemesEndpoint);

        Assert.Contains($"{SourceCodeHelper.Namespace}.UseAuthorizationWithPolicyNameRoleAuthenticationSchemesEndpoint", source);
        Assert.Matches(@"UseAuthorizationWithPolicyNameRoleAuthenticationSchemesEndpoint\.Register\(.*\);", source);
        Assert.Matches(@"UseAuthorizationWithPolicyNameRoleAuthenticationSchemesEndpoint\.Map\(.*\);", source);
        Assert.Matches("""\.RequireAuthorization\(new AuthorizeAttribute { Policy = "auth-policy", Roles = "Admin, User", AuthenticationSchemes = "Bearer, Cookie" }\)""", source);
    }

    [Fact]
    public void WhenEndpointClassMarkUseAuthorizationWithPolicyNameAuthenticationSchemes_ThenEmitConvention() {
        var source = GeneratorTestHelper.GetGeneratedSource(UseAuthorizationWithPolicyNameAuthenticationSchemesEndpoint);

        Assert.Contains($"{SourceCodeHelper.Namespace}.UseAuthorizationWithPolicyNameAuthenticationSchemesEndpoint", source);
        Assert.Matches(@"UseAuthorizationWithPolicyNameAuthenticationSchemesEndpoint\.Register\(.*\);", source);
        Assert.Matches(@"UseAuthorizationWithPolicyNameAuthenticationSchemesEndpoint\.Map\(.*\);", source);
        Assert.Matches("""\.RequireAuthorization\(new AuthorizeAttribute { Policy = "auth-policy", AuthenticationSchemes = "Bearer, Cookie" }\)""", source);
    }
}
