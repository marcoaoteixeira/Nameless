using Nameless.Testing.Tools.Attributes;

namespace Nameless.Generators.Web.Http.Endpoints.EndpointCreation;

[UnitTest]
public class QueryVerbUsageTests
{
    [Fact]
    public void WhenVerbExplicitlySetToQuery_ThenAppliesMapQuery()
    {
        var code = SourceCodeHelper.Write(
            """
            [Endpoint(Verb = HttpVerbs.Query)]
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
        Assert.Matches(@"MapQuery\(.*", source);
    }

    [Fact]
    public void WhenRouteSpecified_ThenMapQueryUsesRouteTemplate()
    {
        var code = SourceCodeHelper.Write(
            """
            [Endpoint("/items", Verb = HttpVerbs.Query)]
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

        Assert.Contains(@"MapQuery(""/items""", source);
    }

    [Fact]
    public void WhenRouteNotSpecified_ThenMapQueryUsesRootRoute()
    {
        var code = SourceCodeHelper.Write(
            """
            [Endpoint(Verb = HttpVerbs.Query)]
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

        Assert.Contains(@"MapQuery(""/""", source);
    }
}
