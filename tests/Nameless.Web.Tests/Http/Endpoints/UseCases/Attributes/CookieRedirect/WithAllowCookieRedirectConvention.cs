using Nameless.Testing.Tools.Attributes;
using Nameless.Web.Http.Endpoints.Infrastructure;

namespace Nameless.Web.Http.Endpoints.UseCases.Attributes.CookieRedirect;

[UnitTest]
public class WithAllowCookieRedirectConvention {
    private readonly string _code;

    public WithAllowCookieRedirectConvention() {
        _code = SourceCodeHelper.Write("""
                                       [Endpoint]
                                       [AllowCookieRedirect]
                                       public partial class AllowCookieRedirectEndpoint {
                                           public Task<IResult> HandleAsync() {
                                               return Task.FromResult<IResult>(TypedResults.Ok());
                                           }
                                       }
                                       """);
    }

    [Fact]
    public void WhenEndpointClassMarkAllowCookieRedirect_ThenEmitConvention() {
        var source = GeneratorTestHelper.GetGeneratedSource(_code);

        Assert.Contains($"{SourceCodeHelper.Namespace}.AllowCookieRedirectEndpoint", source);
        Assert.Matches(@"AllowCookieRedirectEndpoint\.Register\(.*\);", source);
        Assert.Matches(@"AllowCookieRedirectEndpoint\.Map\(.*\);", source);
        Assert.Matches(@"\.AllowCookieRedirect\(\)", source);
    }
}
