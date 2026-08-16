using Nameless.Testing.Tools.Attributes;

namespace Nameless.Generators.Web.Http.Endpoints.Attributes.Cors;

[UnitTest]
public class DisableCorsAttributeUsageTest
{
    [Fact]
    public void WhenGenerateDefault_ThenAppliesCorrectConvention()
    {
        var code = SourceCodeHelper.Write(
            """
            [Endpoint]
            [DisableCors]
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
        Assert.Matches(@"\.WithMetadata\(new DisableCorsAttribute\(\)\)", source);
    }
}
