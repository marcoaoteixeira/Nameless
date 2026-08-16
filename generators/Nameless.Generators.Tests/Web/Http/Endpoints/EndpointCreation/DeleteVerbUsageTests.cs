using Nameless.Testing.Tools.Attributes;

namespace Nameless.Generators.Web.Http.Endpoints.EndpointCreation;

[UnitTest]
public class DeleteVerbUsageTests
{
    [Fact]
    public void WhenVerbExplicitlySetToDelete_ThenAppliesMapDelete()
    {
        var code = SourceCodeHelper.Write(
            """
            [Endpoint(Verb = HttpVerbs.Delete)]
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
        Assert.Matches(@"MapDelete\(.*", source);
    }

    [Fact]
    public void WhenRouteSpecified_ThenMapDeleteUsesRouteTemplate()
    {
        var code = SourceCodeHelper.Write(
            """
            [Endpoint("/items/{id}", Verb = HttpVerbs.Delete)]
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

        Assert.Contains(@"MapDelete(""/items/{id}""", source);
    }

    [Fact]
    public void WhenRouteNotSpecified_ThenMapDeleteUsesRootRoute()
    {
        var code = SourceCodeHelper.Write(
            """
            [Endpoint(Verb = HttpVerbs.Delete)]
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

        Assert.Contains(@"MapDelete(""/""", source);
    }
}
