using Nameless.Testing.Tools.Attributes;
using Nameless.Web.Http.Endpoints.Infrastructure;

namespace Nameless.Web.Http.Endpoints.UseCases.Attributes.Authorization;

[UnitTest]
public class WithAllowAnonymousConvention {
    private readonly string _code;

    public WithAllowAnonymousConvention() {
        _code = SourceCodeHelper.Write("""
                                       [Endpoint]
                                       [AllowAnonymous]
                                       public partial class AllowAnonymousEndpoint {
                                           public Task<IResult> HandleAsync() {
                                               return Task.FromResult<IResult>(TypedResults.Ok("I allow anonymous!"));
                                           }
                                       }
                                       """);
    }

    [Fact]
    public void WhenEndpointClassMarkAllowAnonymous_ThenEmitConvention() {
        var source = GeneratorTestHelper.GetGeneratedSource(_code);

        Assert.Contains($"{SourceCodeHelper.Namespace}.AllowAnonymousEndpoint", source);
        Assert.Matches(@"AllowAnonymousEndpoint\.Register\(.*\);", source);
        Assert.Matches(@"AllowAnonymousEndpoint\.Map\(.*\);", source);
        Assert.Matches(@"\.AllowAnonymous\(\)", source);
    }
}
