using Nameless.Testing.Tools.Attributes;

namespace Nameless.Generators.Web.Http.Endpoints.Attributes.Authorization;

[UnitTest]
public class AuthorizeAttributeUsageTests
{
    [Fact]
    public void WhenGenerateDefault_ThenAppliesCorrectConvention()
    {
        var code = SourceCodeHelper.Write(
            """
            [Endpoint]
            [Authorize]
            public partial class SampleEndpoint {
                public Task<IResult> HandleAsync() {
                    throw new NotImplementedException();
                }
            }
            """
        );

        var source = CodeGeneratorHelper
            .GetCodeBySourceType(code)
            .SingleOrDefault();

        Assert.Contains($"{SourceCodeHelper.Namespace}.SampleEndpoint", source);
        Assert.Matches(@"MapGet\(.*", source);
        Assert.Matches(@"\.RequireAuthorization\(\)", source);
    }

    [Fact]
    public void WhenGenerateWithPolicyInConstructor_ThenAppliesCorrectConvention()
    {
        var code = SourceCodeHelper.Write(
            """
            [Endpoint]
            [Authorize("policy-in-constructor")]
            public partial class SampleEndpoint {
                public Task<IResult> HandleAsync() {
                    throw new NotImplementedException();
                }
            }
            """
        );

        var source = CodeGeneratorHelper
            .GetCodeBySourceType(code)
            .SingleOrDefault();

        Assert.Contains($"{SourceCodeHelper.Namespace}.SampleEndpoint", source);
        Assert.Matches(@"MapGet\(.*", source);
        Assert.Matches(@"\.RequireAuthorization\(new AuthorizeAttribute \{ Policy = ""policy-in-constructor""", source);
    }
    
    [Fact]
    public void WhenGenerateWithPolicyAsParameter_ThenAppliesCorrectConvention()
    {
        var code = SourceCodeHelper.Write(
            """
            [Endpoint]
            [Authorize(Policy = "policy-as-parameter")]
            public partial class SampleEndpoint {
                public Task<IResult> HandleAsync() {
                    throw new NotImplementedException();
                }
            }
            """
        );

        var source = CodeGeneratorHelper
            .GetCodeBySourceType(code)
            .SingleOrDefault();

        Assert.Contains($"{SourceCodeHelper.Namespace}.SampleEndpoint", source);
        Assert.Matches(@"MapGet\(.*", source);
        Assert.Matches(@"\.RequireAuthorization\(new AuthorizeAttribute \{ Policy = ""policy-as-parameter""", source);
    }

    [Fact]
    public void WhenGenerateWithPolicyInConstructorAndAsParameter_ThenPreferParameter_ThenAppliesCorrectConvention()
    {
        var code = SourceCodeHelper.Write(
            """
            [Endpoint]
            [Authorize(policy: "policy-in-constructor", Policy = "policy-as-parameter")]
            public partial class SampleEndpoint {
                public Task<IResult> HandleAsync() {
                    throw new NotImplementedException();
                }
            }
            """
        );

        var source = CodeGeneratorHelper
            .GetCodeBySourceType(code)
            .SingleOrDefault();

        Assert.Contains($"{SourceCodeHelper.Namespace}.SampleEndpoint", source);
        Assert.Matches(@"MapGet\(.*", source);
        Assert.Matches(@"\.RequireAuthorization\(new AuthorizeAttribute \{ Policy = ""policy-as-parameter""", source);
    }

    [Fact]
    public void WhenGenerateWithRolesAsParameter_ThenAppliesCorrectConvention()
    {
        var code = SourceCodeHelper.Write(
            """
            [Endpoint]
            [Authorize(Roles = "role-policy")]
            public partial class SampleEndpoint {
                public Task<IResult> HandleAsync() {
                    throw new NotImplementedException();
                }
            }
            """
        );

        var source = CodeGeneratorHelper
            .GetCodeBySourceType(code)
            .SingleOrDefault();

        Assert.Contains($"{SourceCodeHelper.Namespace}.SampleEndpoint", source);
        Assert.Matches(@"MapGet\(.*", source);
        Assert.Matches("""
                       \.RequireAuthorization\(new AuthorizeAttribute \{ Roles = "role-policy"
                       """, source);
    }

    [Fact]
    public void WhenGenerateWithAuthenticationSchemesAsParameter_ThenAppliesCorrectConvention()
    {
        var code = SourceCodeHelper.Write(
            """
            [Endpoint]
            [Authorize(AuthenticationSchemes = "auth-scheme-policy")]
            public partial class SampleEndpoint {
                public Task<IResult> HandleAsync() {
                    throw new NotImplementedException();
                }
            }
            """
        );

        var source = CodeGeneratorHelper
            .GetCodeBySourceType(code)
            .SingleOrDefault();

        Assert.Contains($"{SourceCodeHelper.Namespace}.SampleEndpoint", source);
        Assert.Matches(@"MapGet\(.*", source);
        Assert.Matches(@"\.RequireAuthorization\(new AuthorizeAttribute \{ AuthenticationSchemes = ""auth-scheme-policy""", source);
    }

    [Fact]
    public void WhenGenerateWithEverything_ThenAppliesCorrectConvention()
    {
        var code = SourceCodeHelper.Write(
            """
            [Endpoint]
            [Authorize(policy: "policy-in-constructor", Policy = "policy-as-parameter", Roles = "role-policy", AuthenticationSchemes = "auth-scheme-policy")]
            public partial class SampleEndpoint {
                public Task<IResult> HandleAsync() {
                    throw new NotImplementedException();
                }
            }
            """
        );

        var source = CodeGeneratorHelper
            .GetCodeBySourceType(code)
            .SingleOrDefault();

        Assert.Contains($"{SourceCodeHelper.Namespace}.SampleEndpoint", source);
        Assert.Matches(@"MapGet\(.*", source);
        Assert.Matches(@"\.RequireAuthorization\(new AuthorizeAttribute \{ Policy = ""policy-as-parameter"", Roles = ""role-policy"", AuthenticationSchemes = ""auth-scheme-policy""", source);
    }
}
