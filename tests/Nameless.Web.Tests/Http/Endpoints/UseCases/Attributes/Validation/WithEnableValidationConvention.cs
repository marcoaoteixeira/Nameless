using Nameless.Testing.Tools.Attributes;
using Nameless.Web.Http.Endpoints.Infrastructure;

namespace Nameless.Web.Http.Endpoints.UseCases.Attributes.Validation;

[UnitTest]
public class WithEnableValidationConvention {
    private readonly string _code;

    public WithEnableValidationConvention() {
        _code = SourceCodeHelper.Write("""
                                       [Endpoint]
                                       [EnableValidation]
                                       public partial class EnableValidationEndpoint {
                                           public Task<IResult> HandleAsync() {
                                               return Task.FromResult<IResult>(TypedResults.Ok());
                                           }
                                       }
                                       """);
    }

    [Fact]
    public void WhenEndpointClassMarkEnableValidation_ThenEmitConvention() {
        var source = GeneratorTestHelper.GetGeneratedSource(_code);

        Assert.Contains($"{SourceCodeHelper.Namespace}.EnableValidationEndpoint", source);
        Assert.Matches(@"EnableValidationEndpoint\.Register\(.*\);", source);
        Assert.Matches(@"EnableValidationEndpoint\.Map\(.*\);", source);
        Assert.Matches(@"\.WithValidation\(\)", source);
    }
}
