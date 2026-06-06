using Nameless.Testing.Tools.Attributes;
using Nameless.Web.Http.Endpoints.Infrastructure;

namespace Nameless.Web.Http.Endpoints.UseCases.Attributes.RequestTimeout;

[UnitTest]
public class WithUseRequestTimeoutConvention {
    private static string UseRequestTimeoutWithPolicyNameEndpoint => SourceCodeHelper.Write(
        """
        [Endpoint<Get>]
        [UseRequestTimeout("my-timeout-policy")]
        public partial class UseRequestTimeoutWithPolicyNameEndpoint {
            public Task<IResult> HandleAsync() {
                return Task.FromResult<IResult>(TypedResults.Ok());
            }
        }
        """
    );

    [Fact]
    public void WhenEndpointClassMarkUseRequestTimeoutWithPolicyName_ThenEmitConvention() {
        var source = GeneratorTestHelper.GetGeneratedSource(UseRequestTimeoutWithPolicyNameEndpoint);

        Assert.Contains($"{SourceCodeHelper.Namespace}.UseRequestTimeoutWithPolicyNameEndpoint", source);
        Assert.Matches(@"UseRequestTimeoutWithPolicyNameEndpoint\.Register\(.*\);", source);
        Assert.Matches(@"UseRequestTimeoutWithPolicyNameEndpoint\.Map\(.*\);", source);
        Assert.Matches(@"\.WithRequestTimeout\(""my-timeout-policy""\)", source);
    }
}
