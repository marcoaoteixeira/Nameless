using Nameless.Testing.Tools.Attributes;
using Nameless.Web.Http.Endpoints.Infrastructure;

namespace Nameless.Web.Http.Endpoints.UseCases.Attributes.Cors;

[UnitTest]
public class WithUseCorsConvention {
    private static string UseCorsWithPolicyNameEndpoint => SourceCodeHelper.Write(
        """
        [Endpoint<Get>]
        [UseCors("my-cors-policy")]
        public partial class UseCorsWithPolicyNameEndpoint {
            public Task<IResult> HandleAsync() {
                return Task.FromResult<IResult>(TypedResults.Ok());
            }
        }
        """
    );

    [Fact]
    public void WhenEndpointClassMarkUseCorsWithPolicyName_ThenEmitConvention() {
        var source = GeneratorTestHelper.GetGeneratedSource(UseCorsWithPolicyNameEndpoint);

        Assert.Contains($"{SourceCodeHelper.Namespace}.UseCorsWithPolicyNameEndpoint", source);
        Assert.Matches(@"UseCorsWithPolicyNameEndpoint\.Register\(.*\);", source);
        Assert.Matches(@"UseCorsWithPolicyNameEndpoint\.Map\(.*\);", source);
        Assert.Matches(@"\.RequireCors\(""my-cors-policy""\)", source);
    }
}
