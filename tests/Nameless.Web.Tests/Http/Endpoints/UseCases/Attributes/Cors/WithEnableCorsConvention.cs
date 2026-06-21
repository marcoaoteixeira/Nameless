using Nameless.Testing.Tools.Attributes;
using Nameless.Web.Http.Endpoints.Infrastructure;

namespace Nameless.Web.Http.Endpoints.UseCases.Attributes.Cors;

[UnitTest]
public class WithEnableCorsConvention {
    private static string EnableCorsWithNoArgumentEndpoint => SourceCodeHelper.Write(
        """
        [Endpoint]
        [EnableCors]
        public partial class EnableCorsWithNoArgumentEndpoint {
            public Task<IResult> HandleAsync() {
                return Task.FromResult<IResult>(TypedResults.Ok());
            }
        }
        """
    );

    private static string EnableCorsWithPolicyNameEndpoint => SourceCodeHelper.Write(
        """
        [Endpoint]
        [EnableCors("my-cors-policy")]
        public partial class EnableCorsWithPolicyNameEndpoint {
            public Task<IResult> HandleAsync() {
                return Task.FromResult<IResult>(TypedResults.Ok());
            }
        }
        """
    );

    [Fact]
    public void WhenEndpointClassMarkEnableCorsWithNoArgument_ThenEmitConvention() {
        var source = GeneratorTestHelper.GetGeneratedSource(EnableCorsWithNoArgumentEndpoint);

        Assert.Contains($"{SourceCodeHelper.Namespace}.EnableCorsWithNoArgumentEndpoint", source);
        Assert.Matches(@"EnableCorsWithNoArgumentEndpoint\.Register\(.*\);", source);
        Assert.Matches(@"EnableCorsWithNoArgumentEndpoint\.Map\(.*\);", source);
        Assert.Matches(@"\.RequireCors\(\)", source);
    }

    [Fact]
    public void WhenEndpointClassMarkEnableCorsWithPolicyName_ThenEmitConvention() {
        var source = GeneratorTestHelper.GetGeneratedSource(EnableCorsWithPolicyNameEndpoint);

        Assert.Contains($"{SourceCodeHelper.Namespace}.EnableCorsWithPolicyNameEndpoint", source);
        Assert.Matches(@"EnableCorsWithPolicyNameEndpoint\.Register\(.*\);", source);
        Assert.Matches(@"EnableCorsWithPolicyNameEndpoint\.Map\(.*\);", source);
        Assert.Matches(@"\.RequireCors\(""my-cors-policy""\)", source);
    }
}
