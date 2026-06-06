using Nameless.Testing.Tools.Attributes;
using Nameless.Web.Http.Endpoints.Infrastructure;

namespace Nameless.Web.Http.Endpoints.UseCases.Attributes.RateLimiting;

[UnitTest]
public class WithDisableRateLimitingConvention {
    private readonly string _code;

    public WithDisableRateLimitingConvention() {
        _code = SourceCodeHelper.Write("""
                                       [Endpoint<Get>]
                                       [DisableRateLimiting]
                                       public partial class DisableRateLimitingEndpoint {
                                           public Task<IResult> HandleAsync() {
                                               return Task.FromResult<IResult>(TypedResults.Ok());
                                           }
                                       }
                                       """);
    }

    [Fact]
    public void WhenEndpointClassMarkDisableRateLimiting_ThenEmitConvention() {
        var source = GeneratorTestHelper.GetGeneratedSource(_code);

        Assert.Contains($"{SourceCodeHelper.Namespace}.DisableRateLimitingEndpoint", source);
        Assert.Matches(@"DisableRateLimitingEndpoint\.Register\(.*\);", source);
        Assert.Matches(@"DisableRateLimitingEndpoint\.Map\(.*\);", source);
        Assert.Matches(@"\.DisableRateLimiting\(\)", source);
    }
}
