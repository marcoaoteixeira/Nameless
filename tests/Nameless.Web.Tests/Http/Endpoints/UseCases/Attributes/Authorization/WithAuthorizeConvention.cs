using Nameless.Testing.Tools.Attributes;
using Nameless.Web.Http.Endpoints.Infrastructure;

namespace Nameless.Web.Http.Endpoints.UseCases.Attributes.Authorization;

[UnitTest]
public class WithAuthorizeConvention {
    private static string AuthorizeWithNoArgumentsEndpoint => SourceCodeHelper.Write(
        """
        [Endpoint<Get>]
        [Authorize]
        public partial class AuthorizeWithNoArgumentsEndpoint {
            public Task<IResult> HandleAsync() {
                return Task.FromResult<IResult>(TypedResults.Ok("OK"));
            }
        }
        """
    );

    private static string AuthorizeWithPolicyEndpoint => SourceCodeHelper.Write(
        """
        [Endpoint<Get>]
        [Authorize(Policy = "auth-policy")]
        public partial class AuthorizeWithPolicyEndpoint {
            public Task<IResult> HandleAsync() {
                return Task.FromResult<IResult>(TypedResults.Ok("OK"));
            }
        }
        """
    );

    private static string AuthorizeWithPolicyRoleEndpoint => SourceCodeHelper.Write(
        """
        [Endpoint<Get>]
        [Authorize(Policy = "auth-policy", Roles = "Admin, User")]
        public partial class AuthorizeWithPolicyRoleEndpoint {
            public Task<IResult> HandleAsync() {
                return Task.FromResult<IResult>(TypedResults.Ok("OK"));
            }
        }
        """
    );

    private static string AuthorizeWithPolicyRoleAuthenticationSchemesEndpoint => SourceCodeHelper.Write(
        """
        [Endpoint<Get>]
        [Authorize(Policy = "auth-policy", Roles = "Admin, User", AuthenticationSchemes = "Bearer, Cookie")]
        public partial class AuthorizeWithPolicyRoleAuthenticationSchemesEndpoint {
            public Task<IResult> HandleAsync() {
                return Task.FromResult<IResult>(TypedResults.Ok("OK"));
            }
        }
        """
    );

    private static string AuthorizeWithPolicyAuthenticationSchemesEndpoint => SourceCodeHelper.Write(
        """
        [Endpoint<Get>]
        [Authorize(Policy = "auth-policy", AuthenticationSchemes = "Bearer, Cookie")]
        public partial class AuthorizeWithPolicyAuthenticationSchemesEndpoint {
            public Task<IResult> HandleAsync() {
                return Task.FromResult<IResult>(TypedResults.Ok("OK"));
            }
        }
        """
    );

    [Fact]
    public void WhenEndpointClassMarkAuthorizeWithNoArguments_ThenEmitConvention() {
        var source = GeneratorTestHelper.GetGeneratedSource(AuthorizeWithNoArgumentsEndpoint);

        Assert.Contains($"{SourceCodeHelper.Namespace}.AuthorizeWithNoArgumentsEndpoint", source);
        Assert.Matches(@"AuthorizeWithNoArgumentsEndpoint\.Register\(.*\);", source);
        Assert.Matches(@"AuthorizeWithNoArgumentsEndpoint\.Map\(.*\);", source);
        Assert.Matches(@"\.RequireAuthorization\(\)", source);
    }

    [Fact]
    public void WhenEndpointClassMarkAuthorizeWithPolicy_ThenEmitConvention() {
        var source = GeneratorTestHelper.GetGeneratedSource(AuthorizeWithPolicyEndpoint);

        Assert.Contains($"{SourceCodeHelper.Namespace}.AuthorizeWithPolicyEndpoint", source);
        Assert.Matches(@"AuthorizeWithPolicyEndpoint\.Register\(.*\);", source);
        Assert.Matches(@"AuthorizeWithPolicyEndpoint\.Map\(.*\);", source);
        Assert.Matches("""\.RequireAuthorization\(new AuthorizeAttribute { Policy = "auth-policy" }\)""", source);
    }

    [Fact]
    public void WhenEndpointClassMarkAuthorizeWithPolicyRole_ThenEmitConvention() {
        var source = GeneratorTestHelper.GetGeneratedSource(AuthorizeWithPolicyRoleEndpoint);

        Assert.Contains($"{SourceCodeHelper.Namespace}.AuthorizeWithPolicyRoleEndpoint", source);
        Assert.Matches(@"AuthorizeWithPolicyRoleEndpoint\.Register\(.*\);", source);
        Assert.Matches(@"AuthorizeWithPolicyRoleEndpoint\.Map\(.*\);", source);
        Assert.Matches("""\.RequireAuthorization\(new AuthorizeAttribute { Policy = "auth-policy", Roles = "Admin, User" }\)""", source);
    }

    [Fact]
    public void WhenEndpointClassMarkAuthorizeWithPolicyRoleAuthenticationSchemes_ThenEmitConvention() {
        var source = GeneratorTestHelper.GetGeneratedSource(AuthorizeWithPolicyRoleAuthenticationSchemesEndpoint);

        Assert.Contains($"{SourceCodeHelper.Namespace}.AuthorizeWithPolicyRoleAuthenticationSchemesEndpoint", source);
        Assert.Matches(@"AuthorizeWithPolicyRoleAuthenticationSchemesEndpoint\.Register\(.*\);", source);
        Assert.Matches(@"AuthorizeWithPolicyRoleAuthenticationSchemesEndpoint\.Map\(.*\);", source);
        Assert.Matches("""\.RequireAuthorization\(new AuthorizeAttribute { Policy = "auth-policy", Roles = "Admin, User", AuthenticationSchemes = "Bearer, Cookie" }\)""", source);
    }

    [Fact]
    public void WhenEndpointClassMarkAuthorizeWithPolicyAuthenticationSchemes_ThenEmitConvention() {
        var source = GeneratorTestHelper.GetGeneratedSource(AuthorizeWithPolicyAuthenticationSchemesEndpoint);

        Assert.Contains($"{SourceCodeHelper.Namespace}.AuthorizeWithPolicyAuthenticationSchemesEndpoint", source);
        Assert.Matches(@"AuthorizeWithPolicyAuthenticationSchemesEndpoint\.Register\(.*\);", source);
        Assert.Matches(@"AuthorizeWithPolicyAuthenticationSchemesEndpoint\.Map\(.*\);", source);
        Assert.Matches("""\.RequireAuthorization\(new AuthorizeAttribute { Policy = "auth-policy", AuthenticationSchemes = "Bearer, Cookie" }\)""", source);
    }
}
