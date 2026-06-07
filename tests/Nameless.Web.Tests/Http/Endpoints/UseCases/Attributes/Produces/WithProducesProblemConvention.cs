using Nameless.Testing.Tools.Attributes;
using Nameless.Web.Http.Endpoints.Infrastructure;

namespace Nameless.Web.Http.Endpoints.UseCases.Attributes.Produces;

[UnitTest]
public class WithProducesProblemConvention {
    private static string ProducesProblemWithDefaultsEndpoint => SourceCodeHelper.Write(
        """
        [Endpoint<Get>]
        [ProducesProblemResponse]
        public partial class ProducesProblemWithDefaultsEndpoint {
            public Task<IResult> HandleAsync() {
                return Task.FromResult<IResult>(TypedResults.Ok());
            }
        }
        """
    );

    private static string ProducesProblemWithCustomStatusCodeEndpoint => SourceCodeHelper.Write(
        """
        [Endpoint<Get>]
        [ProducesProblemResponse(StatusCode = 404)]
        public partial class ProducesProblemWithCustomStatusCodeEndpoint {
            public Task<IResult> HandleAsync() {
                return Task.FromResult<IResult>(TypedResults.Ok());
            }
        }
        """
    );

    [Fact]
    public void WhenEndpointClassMarkProducesProblemWithDefaults_ThenEmitConvention() {
        var source = GeneratorTestHelper.GetGeneratedSource(ProducesProblemWithDefaultsEndpoint);

        Assert.Contains($"{SourceCodeHelper.Namespace}.ProducesProblemWithDefaultsEndpoint", source);
        Assert.Matches(@"ProducesProblemWithDefaultsEndpoint\.Register\(.*\);", source);
        Assert.Matches(@"ProducesProblemWithDefaultsEndpoint\.Map\(.*\);", source);
        Assert.Matches(@"\.ProducesProblem\(statusCode: 500, contentType: ""application/problem\+json""\)", source);
    }

    [Fact]
    public void WhenEndpointClassMarkProducesProblemWithCustomStatusCode_ThenEmitConvention() {
        var source = GeneratorTestHelper.GetGeneratedSource(ProducesProblemWithCustomStatusCodeEndpoint);

        Assert.Contains($"{SourceCodeHelper.Namespace}.ProducesProblemWithCustomStatusCodeEndpoint", source);
        Assert.Matches(@"ProducesProblemWithCustomStatusCodeEndpoint\.Register\(.*\);", source);
        Assert.Matches(@"ProducesProblemWithCustomStatusCodeEndpoint\.Map\(.*\);", source);
        Assert.Matches(@"\.ProducesProblem\(statusCode: 404, contentType: ""application/problem\+json""\)", source);
    }
}
