using Nameless.Testing.Tools.Attributes;

namespace Nameless.Web.Generators.Attributes.RateLimiting;

[UnitTest]
public class EnableRateLimitingAttributeUsageTests
{
    [Fact]
    public void WhenGenerateWithPolicy_ThenAppliesCorrectConvention()
    {
        var code = SourceCodeHelper.Write(
            """
            [Endpoint]
            [EnableRateLimiting("my-rate-limit-policy")]
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
        Assert.Matches(@"\.RequireRateLimiting\(""my-rate-limit-policy""\)", source);
    }
}
