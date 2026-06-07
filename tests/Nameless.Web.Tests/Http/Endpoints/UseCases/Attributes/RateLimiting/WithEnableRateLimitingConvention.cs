using Nameless.Testing.Tools.Attributes;
using Nameless.Web.Http.Endpoints.Infrastructure;

namespace Nameless.Web.Http.Endpoints.UseCases.Attributes.RateLimiting;

[UnitTest]
public class WithEnableRateLimitingConvention {
    private static string EnableRateLimitingWithPolicyNameEndpoint => SourceCodeHelper.Write(
        """
        [Endpoint<Get>]
        [EnableRateLimiting("my-rate-policy")]
        public partial class EnableRateLimitingWithPolicyNameEndpoint {
            public Task<IResult> HandleAsync() {
                return Task.FromResult<IResult>(TypedResults.Ok());
            }
        }
        """
    );

    [Fact]
    public void WhenEndpointClassMarkEnableRateLimitingWithPolicyName_ThenEmitConvention() {
        var source = GeneratorTestHelper.GetGeneratedSource(EnableRateLimitingWithPolicyNameEndpoint);

        Assert.Contains($"{SourceCodeHelper.Namespace}.EnableRateLimitingWithPolicyNameEndpoint", source);
        Assert.Matches(@"EnableRateLimitingWithPolicyNameEndpoint\.Register\(.*\);", source);
        Assert.Matches(@"EnableRateLimitingWithPolicyNameEndpoint\.Map\(.*\);", source);
        Assert.Matches(@"\.RequireRateLimiting\(policyName: ""my-rate-policy""\)", source);
    }
}
