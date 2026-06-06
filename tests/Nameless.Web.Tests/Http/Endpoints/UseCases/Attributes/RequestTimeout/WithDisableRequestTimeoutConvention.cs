using Nameless.Testing.Tools.Attributes;
using Nameless.Web.Http.Endpoints.Infrastructure;

namespace Nameless.Web.Http.Endpoints.UseCases.Attributes.RequestTimeout;

[UnitTest]
public class WithDisableRequestTimeoutConvention {
    private readonly string _code;

    public WithDisableRequestTimeoutConvention() {
        _code = SourceCodeHelper.Write("""
                                       [Endpoint<Get>]
                                       [DisableRequestTimeout]
                                       public partial class DisableRequestTimeoutEndpoint {
                                           public Task<IResult> HandleAsync() {
                                               return Task.FromResult<IResult>(TypedResults.Ok());
                                           }
                                       }
                                       """);
    }

    [Fact]
    public void WhenEndpointClassMarkDisableRequestTimeout_ThenEmitConvention() {
        var source = GeneratorTestHelper.GetGeneratedSource(_code);

        Assert.Contains($"{SourceCodeHelper.Namespace}.DisableRequestTimeoutEndpoint", source);
        Assert.Matches(@"DisableRequestTimeoutEndpoint\.Register\(.*\);", source);
        Assert.Matches(@"DisableRequestTimeoutEndpoint\.Map\(.*\);", source);
        Assert.Matches(@"\.DisableRequestTimeout\(\)", source);
    }
}
