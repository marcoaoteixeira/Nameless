using Nameless.Testing.Tools.Attributes;

namespace Nameless.Generators.Web.Http.Endpoints.Attributes.CookieRedirect;

[UnitTest]
public class DisableCookieRedirectUsageTest {
    private readonly string _code = SourceCodeHelper.Write("""
                                                           [Endpoint]
                                                           [DisableCookieRedirect]
                                                           public partial class SampleEndpoint {
                                                               public Task<IResult> HandleAsync() {
                                                                   return Task.FromResult<IResult>(TypedResults.Ok());
                                                               }
                                                           }
                                                           """);

    [Fact]
    public void WhenEndpointClassMarkDisableCookieRedirect_ThenEmitConvention() {
        var source = CodeGeneratorHelper.GetCodeBySourceType(_code).SingleOrDefault();

        Assert.Contains($"{SourceCodeHelper.Namespace}.SampleEndpoint", source);
        Assert.Matches(@"MapGet\(.*", source);
        Assert.Matches(@"\.DisableCookieRedirect\(\)", source);
    }
}
