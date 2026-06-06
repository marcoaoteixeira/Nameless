using Nameless.Testing.Tools.Attributes;
using Nameless.Web.Http.Endpoints.Infrastructure;

namespace Nameless.Web.Http.Endpoints.UseCases.Attributes.Produces;

[UnitTest]
public class WithProducesValidationProblemConvention {
    private static string ProducesValidationProblemWithDefaultsEndpoint => SourceCodeHelper.Write(
        """
        [Endpoint<Get>]
        [ProducesValidationProblem]
        public partial class ProducesValidationProblemWithDefaultsEndpoint {
            public Task<IResult> HandleAsync() {
                return Task.FromResult<IResult>(TypedResults.Ok());
            }
        }
        """
    );

    private static string ProducesValidationProblemWithCustomStatusCodeEndpoint => SourceCodeHelper.Write(
        """
        [Endpoint<Get>]
        [ProducesValidationProblem(422)]
        public partial class ProducesValidationProblemWithCustomStatusCodeEndpoint {
            public Task<IResult> HandleAsync() {
                return Task.FromResult<IResult>(TypedResults.Ok());
            }
        }
        """
    );

    [Fact]
    public void WhenEndpointClassMarkProducesValidationProblemWithDefaults_ThenEmitConvention() {
        var source = GeneratorTestHelper.GetGeneratedSource(ProducesValidationProblemWithDefaultsEndpoint);

        Assert.Contains($"{SourceCodeHelper.Namespace}.ProducesValidationProblemWithDefaultsEndpoint", source);
        Assert.Matches(@"ProducesValidationProblemWithDefaultsEndpoint\.Register\(.*\);", source);
        Assert.Matches(@"ProducesValidationProblemWithDefaultsEndpoint\.Map\(.*\);", source);
        Assert.Matches(@"\.ProducesValidationProblem\(statusCode: 400, contentType: ""application/problem\+json""\)", source);
    }

    [Fact]
    public void WhenEndpointClassMarkProducesValidationProblemWithCustomStatusCode_ThenEmitConvention() {
        var source = GeneratorTestHelper.GetGeneratedSource(ProducesValidationProblemWithCustomStatusCodeEndpoint);

        Assert.Contains($"{SourceCodeHelper.Namespace}.ProducesValidationProblemWithCustomStatusCodeEndpoint", source);
        Assert.Matches(@"ProducesValidationProblemWithCustomStatusCodeEndpoint\.Register\(.*\);", source);
        Assert.Matches(@"ProducesValidationProblemWithCustomStatusCodeEndpoint\.Map\(.*\);", source);
        Assert.Matches(@"\.ProducesValidationProblem\(statusCode: 422, contentType: ""application/problem\+json""\)", source);
    }
}
