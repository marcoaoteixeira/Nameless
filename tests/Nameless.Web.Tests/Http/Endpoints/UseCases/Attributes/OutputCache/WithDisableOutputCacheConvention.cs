using Nameless.Testing.Tools.Attributes;
using Nameless.Web.Http.Endpoints.Infrastructure;

namespace Nameless.Web.Http.Endpoints.UseCases.Attributes.OutputCache;

[UnitTest]
public class WithDisableOutputCacheConvention {
    private readonly string _code;

    public WithDisableOutputCacheConvention() {
        _code = SourceCodeHelper.Write("""
                                       [Endpoint]
                                       [DisableOutputCache]
                                       public partial class DisableOutputCacheEndpoint {
                                           public Task<IResult> HandleAsync() {
                                               return Task.FromResult<IResult>(TypedResults.Ok());
                                           }
                                       }
                                       """);
    }

    [Fact]
    public void WhenEndpointClassMarkDisableOutputCache_ThenEmitConvention() {
        var source = GeneratorTestHelper.GetGeneratedSource(_code);

        Assert.Contains($"{SourceCodeHelper.Namespace}.DisableOutputCacheEndpoint", source);
        Assert.Matches(@"DisableOutputCacheEndpoint\.Register\(.*\);", source);
        Assert.Matches(@"DisableOutputCacheEndpoint\.Map\(.*\);", source);
        Assert.Matches(@"\.CacheOutput\(policy => policy\.NoCache\(\)\)", source);
    }
}
