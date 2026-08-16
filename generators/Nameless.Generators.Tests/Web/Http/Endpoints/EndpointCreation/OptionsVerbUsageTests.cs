using Nameless.Testing.Tools.Attributes;

namespace Nameless.Generators.Web.Http.Endpoints.EndpointCreation;

[UnitTest]
public class OptionsVerbUsageTests
{
    [Fact]
    public void WhenVerbExplicitlySetToOptions_ThenAppliesMapOptions()
    {
        var code = SourceCodeHelper.Write(
            """
            [Endpoint(Verb = HttpVerbs.Options)]
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
        Assert.Matches(@"MapOptions\(.*", source);
    }

    [Fact]
    public void WhenRouteSpecified_ThenMapOptionsUsesRouteTemplate()
    {
        var code = SourceCodeHelper.Write(
            """
            [Endpoint("/items/{id}", Verb = HttpVerbs.Options)]
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

        Assert.Contains(@"MapOptions(""/items/{id}""", source);
    }

    [Fact]
    public void WhenRouteNotSpecified_ThenMapOptionsUsesRootRoute()
    {
        var code = SourceCodeHelper.Write(
            """
            [Endpoint(Verb = HttpVerbs.Options)]
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

        Assert.Contains(@"MapOptions(""/""", source);
    }
}
