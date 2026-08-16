using Nameless.Testing.Tools.Attributes;

namespace Nameless.Generators.Web.Http.Endpoints.EndpointCreation;

[UnitTest]
public class GetVerbUsageTests
{
    [Fact]
    public void WhenNoVerbSpecified_ThenDefaultsToMapGet()
    {
        var code = SourceCodeHelper.Write(
            """
            [Endpoint]
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
        Assert.Matches(@"MapGet\(.*", source);
    }

    [Fact]
    public void WhenVerbExplicitlySetToGet_ThenAppliesMapGet()
    {
        var code = SourceCodeHelper.Write(
            """
            [Endpoint(Verb = HttpVerbs.Get)]
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
        Assert.Matches(@"MapGet\(.*", source);
    }

    [Fact]
    public void WhenRouteSpecified_ThenMapGetUsesRouteTemplate()
    {
        var code = SourceCodeHelper.Write(
            """
            [Endpoint("/items/{id}")]
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

        Assert.Contains(@"MapGet(""/items/{id}""", source);
    }

    [Fact]
    public void WhenRouteNotSpecified_ThenMapGetUsesRootRoute()
    {
        var code = SourceCodeHelper.Write(
            """
            [Endpoint]
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

        Assert.Contains(@"MapGet(""/""", source);
    }
}
