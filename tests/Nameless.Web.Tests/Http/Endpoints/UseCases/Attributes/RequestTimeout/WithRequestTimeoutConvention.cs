using Nameless.Testing.Tools.Attributes;
using Nameless.Web.Http.Endpoints.Infrastructure;

namespace Nameless.Web.Http.Endpoints.UseCases.Attributes.RequestTimeout;

[UnitTest]
public class WithRequestTimeoutConvention {
    private static string RequestTimeoutWithMillisecondsEndpoint => SourceCodeHelper.Write(
        """
        [Endpoint]
        [RequestTimeout(milliseconds: 1234)]
        public partial class RequestTimeoutWithPolicyNameEndpoint {
            public Task<IResult> HandleAsync() {
                return Task.FromResult<IResult>(TypedResults.Ok());
            }
        }
        """
    );

    private static string RequestTimeoutWithPolicyNameEndpoint => SourceCodeHelper.Write(
        """
        [Endpoint]
        [RequestTimeout("my-timeout-policy")]
        public partial class RequestTimeoutWithPolicyNameEndpoint {
            public Task<IResult> HandleAsync() {
                return Task.FromResult<IResult>(TypedResults.Ok());
            }
        }
        """
    );

    [Fact]
    public void WhenEndpointClassMarkRequestTimeoutWithMilliseconds_ThenEmitConvention() {
        var source = GeneratorTestHelper.GetGeneratedSource(RequestTimeoutWithMillisecondsEndpoint);

        Assert.Contains($"{SourceCodeHelper.Namespace}.RequestTimeoutWithPolicyNameEndpoint", source);
        Assert.Matches(@"RequestTimeoutWithPolicyNameEndpoint\.Register\(.*\);", source);
        Assert.Matches(@"RequestTimeoutWithPolicyNameEndpoint\.Map\(.*\);", source);
        Assert.Matches(@"\.WithRequestTimeout\(timeout: TimeSpan.FromMilliseconds\(1234\)\)", source);
    }

    [Fact]
    public void WhenEndpointClassMarkRequestTimeoutWithPolicyName_ThenEmitConvention() {
        var source = GeneratorTestHelper.GetGeneratedSource(RequestTimeoutWithPolicyNameEndpoint);

        Assert.Contains($"{SourceCodeHelper.Namespace}.RequestTimeoutWithPolicyNameEndpoint", source);
        Assert.Matches(@"RequestTimeoutWithPolicyNameEndpoint\.Register\(.*\);", source);
        Assert.Matches(@"RequestTimeoutWithPolicyNameEndpoint\.Map\(.*\);", source);
        Assert.Matches(@"\.WithRequestTimeout\(policyName: ""my-timeout-policy""\)", source);
    }
}
