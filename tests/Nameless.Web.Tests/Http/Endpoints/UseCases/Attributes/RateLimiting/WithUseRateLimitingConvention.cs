using Nameless.Testing.Tools.Attributes;
using Nameless.Web.Http.Endpoints.Infrastructure;

namespace Nameless.Web.Http.Endpoints.UseCases.Attributes.RateLimiting;

[UnitTest]
public class WithUseRateLimitingConvention {
    private static string UseRateLimitingWithPolicyNameEndpoint => SourceCodeHelper.Write(
        """
        [Endpoint<Get>]
        [UseRateLimiting("my-rate-policy")]
        public partial class UseRateLimitingWithPolicyNameEndpoint {
            public Task<IResult> HandleAsync() {
                return Task.FromResult<IResult>(TypedResults.Ok());
            }
        }
        """
    );

    [Fact]
    public void WhenEndpointClassMarkUseRateLimitingWithPolicyName_ThenEmitConvention() {
        var source = GeneratorTestHelper.GetGeneratedSource(UseRateLimitingWithPolicyNameEndpoint);

        Assert.Contains($"{SourceCodeHelper.Namespace}.UseRateLimitingWithPolicyNameEndpoint", source);
        Assert.Matches(@"UseRateLimitingWithPolicyNameEndpoint\.Register\(.*\);", source);
        Assert.Matches(@"UseRateLimitingWithPolicyNameEndpoint\.Map\(.*\);", source);
        Assert.Matches(@"\.RequireRateLimiting\(""my-rate-policy""\)", source);
    }
}
