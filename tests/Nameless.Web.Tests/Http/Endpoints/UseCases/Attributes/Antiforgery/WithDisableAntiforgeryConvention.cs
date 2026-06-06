using Nameless.Testing.Tools.Attributes;
using Nameless.Web.Http.Endpoints.Infrastructure;

namespace Nameless.Web.Http.Endpoints.UseCases.Attributes.Antiforgery;

[UnitTest]
public class WithDisableAntiforgeryConvention {
    private readonly string _code;

    public WithDisableAntiforgeryConvention() {
        _code = SourceCodeHelper.Write("""
                                       [Endpoint<Get>]
                                       [DisableAntiforgery]
                                       public partial class DisableAntiforgeryEndpoint {
                                           public Task<IResult> HandleAsync() {
                                               return Task.FromResult<IResult>(TypedResults.Ok());
                                           }
                                       }
                                       """);
    }

    [Fact]
    public void WhenEndpointClassMarkDisableAntiforgery_ThenEmitConvention() {
        var source = GeneratorTestHelper.GetGeneratedSource(_code);

        Assert.Contains($"{SourceCodeHelper.Namespace}.DisableAntiforgeryEndpoint", source);
        Assert.Matches(@"DisableAntiforgeryEndpoint\.Register\(.*\);", source);
        Assert.Matches(@"DisableAntiforgeryEndpoint\.Map\(.*\);", source);
        Assert.Matches(@"\.DisableAntiforgery\(\)", source);
    }
}
