using Nameless.Testing.Tools.Attributes;

namespace Nameless.Generators.Web.Http.Endpoints.Attributes.Versioning;

[UnitTest]
public class DeprecateAttributeUsageTests
{
    [Fact]
    public void WhenGenerateDefault_ThenAppliesCorrectConvention()
    {
        var code = SourceCodeHelper.Write(
            """
            [Endpoint]
            [Deprecate]
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
        Assert.Matches(@"\.AddOpenApiOperationTransformer\(", source);
        Assert.Matches(@"op\.Deprecated = true", source);
    }

    [Fact]
    public void WhenGenerateWithMessage_ThenIncludesDescriptionInConvention()
    {
        var code = SourceCodeHelper.Write(
            """
            [Endpoint]
            [Deprecate(Message = "Use v2 instead.")]
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
        Assert.Matches(@"op\.Description = ""Use v2 instead\.""", source);
    }

    [Fact]
    public void WhenGenerateWithSunset_ThenIncludesSunsetInConvention()
    {
        var code = SourceCodeHelper.Write(
            """
            [Endpoint]
            [Deprecate(Sunset = "Fri, 31 Dec 2027 23:59:59 GMT")]
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
        Assert.Matches(@"\.WithSunset\(sunsetDate: ""Fri, 31 Dec 2027 23:59:59 GMT""", source);
    }

    [Fact]
    public void WhenGenerateWithSunsetAndLink_ThenIncludesBothInConvention()
    {
        var code = SourceCodeHelper.Write(
            """
            [Endpoint]
            [Deprecate(Sunset = "Fri, 31 Dec 2027 23:59:59 GMT", Link = "https://example.com/migration")]
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
        Assert.Matches(@"sunsetDate: ""Fri, 31 Dec 2027 23:59:59 GMT""", source);
        Assert.Matches(@"link: ""https://example\.com/migration""", source);
    }
}
