using Nameless.Testing.Tools.Attributes;

namespace Nameless.Generators.Web.Http.Endpoints.Attributes.Produces;

[UnitTest]
public class ProducesValidationProblemResponseAttributeUsageTest
{
    [Fact]
    public void WhenGenerateDefault_ThenAppliesCorrectConvention()
    {
        var code = SourceCodeHelper.Write(
            """
            [Endpoint]
            [ProducesValidationProblemResponse]
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
        Assert.Matches(@"\.ProducesValidationProblem\(statusCode: 400, contentType: ""application/problem\+json""\)", source);
    }

    [Fact]
    public void WhenGenerateWithCustomStatusCode_ThenAppliesCorrectConvention()
    {
        var code = SourceCodeHelper.Write(
            """
            [Endpoint]
            [ProducesValidationProblemResponse(StatusCode = 422)]
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
        Assert.Matches(@"\.ProducesValidationProblem\(statusCode: 422,", source);
    }
}
