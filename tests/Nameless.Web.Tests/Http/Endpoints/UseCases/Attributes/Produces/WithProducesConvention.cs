using Nameless.Testing.Tools.Attributes;
using Nameless.Web.Http.Endpoints.Infrastructure;

namespace Nameless.Web.Http.Endpoints.UseCases.Attributes.Produces;

[UnitTest]
public class WithProducesConvention {
    private static string ProducesWithGenericSyntaxDefaultsEndpoint => SourceCodeHelper.Write(
        """
        [Endpoint<Get>]
        [Produces<string>]
        public partial class ProducesWithGenericSyntaxDefaultsEndpoint {
            public Task<IResult> HandleAsync() {
                return Task.FromResult<IResult>(TypedResults.Ok("OK"));
            }
        }
        """
    );

    private static string ProducesWithGenericSyntaxCustomStatusCodeEndpoint => SourceCodeHelper.Write(
        """
        [Endpoint<Post>]
        [Produces<string>(201)]
        public partial class ProducesWithGenericSyntaxCustomStatusCodeEndpoint {
            public Task<IResult> HandleAsync() {
                return Task.FromResult<IResult>(TypedResults.Created());
            }
        }
        """
    );

    private static string ProducesWithTypeofSyntaxEndpoint => SourceCodeHelper.Write(
        """
        [Endpoint<Get>]
        [Produces(typeof(string), 200)]
        public partial class ProducesWithTypeofSyntaxEndpoint {
            public Task<IResult> HandleAsync() {
                return Task.FromResult<IResult>(TypedResults.Ok("OK"));
            }
        }
        """
    );

    [Fact]
    public void WhenEndpointClassMarkProducesWithGenericSyntaxDefaults_ThenEmitConvention() {
        var source = GeneratorTestHelper.GetGeneratedSource(ProducesWithGenericSyntaxDefaultsEndpoint);

        Assert.Contains($"{SourceCodeHelper.Namespace}.ProducesWithGenericSyntaxDefaultsEndpoint", source);
        Assert.Matches(@"ProducesWithGenericSyntaxDefaultsEndpoint\.Register\(.*\);", source);
        Assert.Matches(@"ProducesWithGenericSyntaxDefaultsEndpoint\.Map\(.*\);", source);
        Assert.Matches(@"\.Produces<string>\(statusCode: 200, contentType: ""application/json""\)", source);
    }

    [Fact]
    public void WhenEndpointClassMarkProducesWithGenericSyntaxCustomStatusCode_ThenEmitConvention() {
        var source = GeneratorTestHelper.GetGeneratedSource(ProducesWithGenericSyntaxCustomStatusCodeEndpoint);

        Assert.Contains($"{SourceCodeHelper.Namespace}.ProducesWithGenericSyntaxCustomStatusCodeEndpoint", source);
        Assert.Matches(@"ProducesWithGenericSyntaxCustomStatusCodeEndpoint\.Register\(.*\);", source);
        Assert.Matches(@"ProducesWithGenericSyntaxCustomStatusCodeEndpoint\.Map\(.*\);", source);
        Assert.Matches(@"\.Produces<string>\(statusCode: 201, contentType: ""application/json""\)", source);
    }

    [Fact]
    public void WhenEndpointClassMarkProducesWithTypeofSyntax_ThenEmitConvention() {
        var source = GeneratorTestHelper.GetGeneratedSource(ProducesWithTypeofSyntaxEndpoint);

        Assert.Contains($"{SourceCodeHelper.Namespace}.ProducesWithTypeofSyntaxEndpoint", source);
        Assert.Matches(@"ProducesWithTypeofSyntaxEndpoint\.Register\(.*\);", source);
        Assert.Matches(@"ProducesWithTypeofSyntaxEndpoint\.Map\(.*\);", source);
        Assert.Matches(@"\.Produces<string>\(statusCode: 200, contentType: ""application/json""\)", source);
    }
}
