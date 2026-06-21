using Nameless.Testing.Tools.Attributes;
using Nameless.Web.Http.Endpoints.Infrastructure;

namespace Nameless.Web.Http.Endpoints.UseCases.Attributes.Cors;

[UnitTest]
public class WithDisableCorsConvention {
    private readonly string _code;

    public WithDisableCorsConvention() {
        _code = SourceCodeHelper.Write("""
                                       [Endpoint]
                                       [DisableCors]
                                       public partial class DisableCorsEndpoint {
                                           public Task<IResult> HandleAsync() {
                                               return Task.FromResult<IResult>(TypedResults.Ok());
                                           }
                                       }
                                       """);
    }

    [Fact]
    public void WhenEndpointClassMarkDisableCors_ThenEmitConvention() {
        var source = GeneratorTestHelper.GetGeneratedSource(_code);

        Assert.Contains($"{SourceCodeHelper.Namespace}.DisableCorsEndpoint", source);
        Assert.Matches(@"DisableCorsEndpoint\.Register\(.*\);", source);
        Assert.Matches(@"DisableCorsEndpoint\.Map\(.*\);", source);
        Assert.Matches(@"\.WithMetadata\(new DisableCorsAttribute\(\)\)", source);
    }
}
