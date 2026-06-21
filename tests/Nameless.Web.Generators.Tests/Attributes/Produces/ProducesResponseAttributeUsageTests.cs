using Nameless.Testing.Tools.Attributes;

namespace Nameless.Web.Generators.Attributes.Produces;

[UnitTest]
public class ProducesResponseAttributeUsageTests
{
    [Fact]
    public void WhenGenerateWithGenericTypeArgument_ThenAppliesCorrectConvention()
    {
        var code = SourceCodeHelper.Write(
            """
            public record SampleModel(string Name);

            [Endpoint]
            [ProducesResponse<SampleModel>]
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
        Assert.Contains(".Produces<", source);
        Assert.Contains("SampleModel", source);
        Assert.Contains("statusCode: 200", source);
        Assert.Contains(@"contentType: ""application/json""", source);
    }

    [Fact]
    public void WhenGenerateWithCustomStatusCode_ThenAppliesCorrectConvention()
    {
        var code = SourceCodeHelper.Write(
            """
            public record SampleModel(string Name);

            [Endpoint(Verb = HttpVerbs.Post)]
            [ProducesResponse<SampleModel>(StatusCode = 201)]
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
        Assert.Matches(@"MapPost\(.*", source);
        Assert.Contains(".Produces<", source);
        Assert.Contains("SampleModel", source);
        Assert.Contains("statusCode: 201", source);
    }

    [Fact]
    public void WhenGenerateWithCustomContentType_ThenAppliesCorrectConvention()
    {
        var code = SourceCodeHelper.Write(
            """
            public record SampleModel(string Name);

            [Endpoint]
            [ProducesResponse<SampleModel>(ContentType = "text/plain")]
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
        Assert.Contains(".Produces<", source);
        Assert.Contains(@"contentType: ""text/plain""", source);
    }
}