using Nameless.Testing.Tools.Attributes;

namespace Nameless.Web.Generators.Attributes.Antiforgery;

[UnitTest]
public class DisableAntiforgeryUsageTest {
    private readonly string _code;

    public DisableAntiforgeryUsageTest() {
        _code = SourceCodeHelper.Write("""
                                       [Endpoint]
                                       [DisableAntiforgery]
                                       public partial class SampleEndpoint {
                                           public Task<IResult> HandleAsync() {
                                               return Task.FromResult<IResult>(TypedResults.Ok());
                                           }
                                       }
                                       """);
    }

    [Fact]
    public void WhenEndpointClassMarkDisableAntiforgery_ThenEmitConvention() {
        var source = CodeGeneratorHelper.GetCodeBySourceType(_code).SingleOrDefault();

        Assert.Matches(@"\.DisableAntiforgery\(\)", source);
    }
}
