using Nameless.Testing.Tools.Attributes;

namespace Nameless.Web.Generators.Attributes.RequestTimeout;

[UnitTest]
public class RequestTimeoutAttributeUsageTests
{
    [Fact]
    public void WhenGenerateWithPolicy_ThenAppliesCorrectConvention()
    {
        var code = SourceCodeHelper.Write(
            """
            [Endpoint]
            [RequestTimeout("my-timeout-policy")]
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
        Assert.Matches(@"\.WithRequestTimeout\(""my-timeout-policy""\)", source);
    }
}
