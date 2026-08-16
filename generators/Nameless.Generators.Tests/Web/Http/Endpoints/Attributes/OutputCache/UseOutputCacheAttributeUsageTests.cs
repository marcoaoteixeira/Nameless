using Nameless.Testing.Tools.Attributes;

namespace Nameless.Generators.Web.Http.Endpoints.Attributes.OutputCache;

[UnitTest]
public class UseOutputCacheAttributeUsageTests
{
    [Fact]
    public void WhenGenerateDefault_ThenAppliesCorrectConvention()
    {
        var code = SourceCodeHelper.Write(
            """
            [Endpoint]
            [UseOutputCache]
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
        Assert.Matches(@"\.CacheOutput\(\)", source);
    }

    [Fact]
    public void WhenGenerateWithNoStore_ThenAppliesNoCacheConvention()
    {
        var code = SourceCodeHelper.Write(
            """
            [Endpoint]
            [UseOutputCache(NoStore = true)]
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
        Assert.Matches(@"\.CacheOutput\(policy => policy\.NoCache\(\)\)", source);
    }

    [Fact]
    public void WhenGenerateWithPolicyName_ThenAppliesCorrectConvention()
    {
        var code = SourceCodeHelper.Write(
            """
            [Endpoint]
            [UseOutputCache(PolicyName = "my-cache-policy")]
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
        Assert.Matches(@"\.CacheOutput\(""my-cache-policy""\)", source);
    }

    [Fact]
    public void WhenGenerateWithDuration_ThenAppliesCorrectConvention()
    {
        var code = SourceCodeHelper.Write(
            """
            [Endpoint]
            [UseOutputCache(Duration = 60)]
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
        Assert.Matches(@"\.CacheOutput\(policy =>", source);
        Assert.Matches(@"policy\.Cache\(\)", source);
        Assert.Matches(@"policy\.Expire\(TimeSpan\.FromSeconds\(60\)\)", source);
    }

    [Fact]
    public void WhenGenerateWithVaryByQueryKeys_ThenAppliesCorrectConvention()
    {
        var code = SourceCodeHelper.Write(
            """
            [Endpoint]
            [UseOutputCache(Duration = 30, VaryByQueryKeys = new string[] { "page", "size" })]
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
        Assert.Matches(@"policy\.SetVaryByQuery\(""page"", ""size""\)", source);
    }

    [Fact]
    public void WhenGenerateWithVaryByHeaderNames_ThenAppliesCorrectConvention()
    {
        var code = SourceCodeHelper.Write(
            """
            [Endpoint]
            [UseOutputCache(Duration = 30, VaryByHeaderNames = new string[] { "Accept-Language" })]
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
        Assert.Matches(@"policy\.SetVaryByHeader\(""Accept-Language""\)", source);
    }

    [Fact]
    public void WhenGenerateWithTags_ThenAppliesCorrectConvention()
    {
        var code = SourceCodeHelper.Write(
            """
            [Endpoint]
            [UseOutputCache(Duration = 30, Tags = new string[] { "products" })]
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
        Assert.Matches(@"policy\.Tag\(""products""\)", source);
    }
}
