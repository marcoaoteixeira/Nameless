using Nameless.Testing.Tools.Attributes;

namespace Nameless.Generators.Web.Http.Endpoints.EndpointCreation;

[UnitTest]
public class HeadVerbUsageTests
{
    [Fact]
    public void WhenVerbExplicitlySetToHead_ThenAppliesMapHead()
    {
        var code = SourceCodeHelper.Write(
            """
            [Endpoint(Verb = HttpVerbs.Head)]
            public partial class SampleEndpoint {
                public Task<IResult> HandleAsync() {
                    throw new NotImplementedException();
                }
            }
            """
        );

        var source = CodeGeneratorHelper
            .GetCodeBySourceType(code)
            .SingleOrDefault();

        Assert.Contains($"{SourceCodeHelper.Namespace}.SampleEndpoint", source);
        Assert.Matches(@"MapHead\(.*", source);
    }

    [Fact]
    public void WhenRouteSpecified_ThenMapHeadUsesRouteTemplate()
    {
        var code = SourceCodeHelper.Write(
            """
            [Endpoint("/items/{id}", Verb = HttpVerbs.Head)]
            public partial class SampleEndpoint {
                public Task<IResult> HandleAsync() {
                    throw new NotImplementedException();
                }
            }
            """
        );

        var source = CodeGeneratorHelper
            .GetCodeBySourceType(code)
            .SingleOrDefault();

        Assert.Contains(@"MapHead(""/items/{id}""", source);
    }

    [Fact]
    public void WhenRouteNotSpecified_ThenMapHeadUsesRootRoute()
    {
        var code = SourceCodeHelper.Write(
            """
            [Endpoint(Verb = HttpVerbs.Head)]
            public partial class SampleEndpoint {
                public Task<IResult> HandleAsync() {
                    throw new NotImplementedException();
                }
            }
            """
        );

        var source = CodeGeneratorHelper
            .GetCodeBySourceType(code)
            .SingleOrDefault();

        Assert.Contains(@"MapHead(""/""", source);
    }
}
