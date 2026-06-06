using Nameless.Testing.Tools.Attributes;
using Nameless.Web.Http.Endpoints.Infrastructure;

namespace Nameless.Web.Http.Endpoints.UseCases.Attributes.OutputCache;

[UnitTest]
public class WithUseOutputCacheConvention {
    private static string UseOutputCacheWithPolicyNameEndpoint => SourceCodeHelper.Write(
        """
        [Endpoint<Get>]
        [UseOutputCache("my-cache-policy")]
        public partial class UseOutputCacheWithPolicyNameEndpoint {
            public Task<IResult> HandleAsync() {
                return Task.FromResult<IResult>(TypedResults.Ok());
            }
        }
        """
    );

    [Fact]
    public void WhenEndpointClassMarkUseOutputCacheWithPolicyName_ThenEmitConvention() {
        var source = GeneratorTestHelper.GetGeneratedSource(UseOutputCacheWithPolicyNameEndpoint);

        Assert.Contains($"{SourceCodeHelper.Namespace}.UseOutputCacheWithPolicyNameEndpoint", source);
        Assert.Matches(@"UseOutputCacheWithPolicyNameEndpoint\.Register\(.*\);", source);
        Assert.Matches(@"UseOutputCacheWithPolicyNameEndpoint\.Map\(.*\);", source);
        Assert.Matches(@"\.CacheOutput\(""my-cache-policy""\)", source);
    }
}
