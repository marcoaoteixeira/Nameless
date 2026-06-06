using Nameless.Testing.Tools.Attributes;
using Nameless.Web.Http.Endpoints.Infrastructure;

namespace Nameless.Web.Http.Endpoints.UseCases.Attributes.CookieRedirect;

[UnitTest]
public class WithDisableCookieRedirectConvention {
    private readonly string _code;

    public WithDisableCookieRedirectConvention() {
        _code = SourceCodeHelper.Write("""
                                       [Endpoint<Get>]
                                       [DisableCookieRedirect]
                                       public partial class DisableCookieRedirectEndpoint {
                                           public Task<IResult> HandleAsync() {
                                               return Task.FromResult<IResult>(TypedResults.Ok());
                                           }
                                       }
                                       """);
    }

    [Fact]
    public void WhenEndpointClassMarkDisableCookieRedirect_ThenEmitConvention() {
        var source = GeneratorTestHelper.GetGeneratedSource(_code);

        Assert.Contains($"{SourceCodeHelper.Namespace}.DisableCookieRedirectEndpoint", source);
        Assert.Matches(@"DisableCookieRedirectEndpoint\.Register\(.*\);", source);
        Assert.Matches(@"DisableCookieRedirectEndpoint\.Map\(.*\);", source);
        Assert.Matches(@"\.DisableCookieRedirect\(\)", source);
    }
}
