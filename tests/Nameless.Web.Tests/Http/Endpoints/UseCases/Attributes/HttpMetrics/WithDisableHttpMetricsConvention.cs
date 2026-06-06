using Nameless.Testing.Tools.Attributes;
using Nameless.Web.Http.Endpoints.Infrastructure;

namespace Nameless.Web.Http.Endpoints.UseCases.Attributes.HttpMetrics;

[UnitTest]
public class WithDisableHttpMetricsConvention {
    private readonly string _code;

    public WithDisableHttpMetricsConvention() {
        _code = SourceCodeHelper.Write("""
                                       [Endpoint<Get>]
                                       [DisableHttpMetrics]
                                       public partial class DisableHttpMetricsEndpoint {
                                           public Task<IResult> HandleAsync() {
                                               return Task.FromResult<IResult>(TypedResults.Ok());
                                           }
                                       }
                                       """);
    }

    [Fact]
    public void WhenEndpointClassMarkDisableHttpMetrics_ThenEmitConvention() {
        var source = GeneratorTestHelper.GetGeneratedSource(_code);

        Assert.Contains($"{SourceCodeHelper.Namespace}.DisableHttpMetricsEndpoint", source);
        Assert.Matches(@"DisableHttpMetricsEndpoint\.Register\(.*\);", source);
        Assert.Matches(@"DisableHttpMetricsEndpoint\.Map\(.*\);", source);
        Assert.Matches(@"\.DisableHttpMetrics\(\)", source);
    }
}
