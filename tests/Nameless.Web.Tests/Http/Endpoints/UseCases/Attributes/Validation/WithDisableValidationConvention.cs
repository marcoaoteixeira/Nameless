using Nameless.Testing.Tools.Attributes;
using Nameless.Web.Http.Endpoints.Infrastructure;

namespace Nameless.Web.Http.Endpoints.UseCases.Attributes.Validation;

[UnitTest]
public class WithDisableValidationConvention {
    private readonly string _code;

    public WithDisableValidationConvention() {
        _code = SourceCodeHelper.Write("""
                                       [Endpoint]
                                       [DisableValidator]
                                       public partial class DisableValidationEndpoint {
                                           public Task<IResult> HandleAsync() {
                                               return Task.FromResult<IResult>(TypedResults.Ok());
                                           }
                                       }
                                       """);
    }

    [Fact]
    public void WhenEndpointClassMarkDisableValidation_ThenEmitConvention() {
        var source = GeneratorTestHelper.GetGeneratedSource(_code);

        Assert.Contains($"{SourceCodeHelper.Namespace}.DisableValidationEndpoint", source);
        Assert.Matches(@"DisableValidationEndpoint\.Register\(.*\);", source);
        Assert.Matches(@"DisableValidationEndpoint\.Map\(.*\);", source);
        Assert.Matches(@"\.DisableValidator\(\)", source);
    }
}
