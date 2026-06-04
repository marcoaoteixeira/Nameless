using Nameless.Testing.Tools.Attributes;
using Nameless.Web.Http.Endpoints.Infrastructure;

namespace Nameless.Web.Http.Endpoints.UseCases;

[UnitTest]
public class SimpleGetEndpoint {
    private readonly string _code;

    public SimpleGetEndpoint() {
        _code = SourceCodeHelper.Write("""
                                       [Endpoint<Get>]
                                       public partial class HelloWorldEndpoint {
                                           public Task<IResult> HandleAsync() {
                                               return Task.FromResult<IResult>(TypedResults.Ok("Hello World!"));
                                           }
                                       }
                                       """);
    }

    [Fact]
    public void WhenDeclareClassWithEndpointAttribute_ThenEmitCorrectMapMethod() {
        var source = GeneratorTestHelper.GetGeneratedSource(_code);

        Assert.Contains($"{SourceCodeHelper.Namespace}.HelloWorldEndpoint", source);
        Assert.Matches(@"HelloWorldEndpoint\.Register\(.*\);", source);
        Assert.Matches(@"HelloWorldEndpoint\.Map\(.*\);", source);
    }
}
