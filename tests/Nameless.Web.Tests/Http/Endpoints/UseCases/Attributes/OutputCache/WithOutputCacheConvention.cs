using Nameless.Testing.Tools.Attributes;
using Nameless.Web.Http.Endpoints.Infrastructure;

namespace Nameless.Web.Http.Endpoints.UseCases.Attributes.OutputCache;

[UnitTest]
public class WithOutputCacheConvention {
    private static string OutputCacheWithNoArgsEndpoint => SourceCodeHelper.Write(
        """
        [Endpoint<Get>]
        [OutputCache]
        public partial class OutputCacheWithNoArgsEndpoint {
            public Task<IResult> HandleAsync() {
                return Task.FromResult<IResult>(TypedResults.Ok());
            }
        }
        """
    );

    private static string OutputCacheWithNoStoreEndpoint => SourceCodeHelper.Write(
        """
        [Endpoint<Get>]
        [OutputCache(NoStore = true)]
        public partial class OutputCacheWithNoStoreEndpoint {
            public Task<IResult> HandleAsync() {
                return Task.FromResult<IResult>(TypedResults.Ok());
            }
        }
        """
    );

    private static string OutputCacheWithPolicyNameEndpoint => SourceCodeHelper.Write(
        """
        [Endpoint<Get>]
        [OutputCache(PolicyName = "my-cache-policy")]
        public partial class OutputCacheWithPolicyNameEndpoint {
            public Task<IResult> HandleAsync() {
                return Task.FromResult<IResult>(TypedResults.Ok());
            }
        }
        """
    );

    private static string OutputCacheWithDurationEndpoint => SourceCodeHelper.Write(
        """
        [Endpoint<Get>]
        [OutputCache(Duration = 60)]
        public partial class OutputCacheWithDurationEndpoint {
            public Task<IResult> HandleAsync() {
                return Task.FromResult<IResult>(TypedResults.Ok());
            }
        }
        """
    );

    private static string OutputCacheWithVaryByQueryKeysEndpoint => SourceCodeHelper.Write(
        """
        [Endpoint<Get>]
        [OutputCache(Duration = 120, VaryByQueryKeys = new[] { "search", "page" })]
        public partial class OutputCacheWithVaryByQueryKeysEndpoint {
            public Task<IResult> HandleAsync() {
                return Task.FromResult<IResult>(TypedResults.Ok());
            }
        }
        """
    );

    private static string OutputCacheWithTagsEndpoint => SourceCodeHelper.Write(
        """
        [Endpoint<Get>]
        [OutputCache(Duration = 60, Tags = new[] { "products" })]
        public partial class OutputCacheWithTagsEndpoint {
            public Task<IResult> HandleAsync() {
                return Task.FromResult<IResult>(TypedResults.Ok());
            }
        }
        """
    );

    [Fact]
    public void WhenEndpointClassMarkOutputCacheWithNoArgs_ThenEmitCacheOutputConvention() {
        var source = GeneratorTestHelper.GetGeneratedSource(OutputCacheWithNoArgsEndpoint);

        Assert.Contains($"{SourceCodeHelper.Namespace}.OutputCacheWithNoArgsEndpoint", source);
        Assert.Matches(@"OutputCacheWithNoArgsEndpoint\.Register\(.*\);", source);
        Assert.Matches(@"OutputCacheWithNoArgsEndpoint\.Map\(.*\);", source);
        Assert.Matches(@"\.CacheOutput\(\)", source);
    }

    [Fact]
    public void WhenEndpointClassMarkOutputCacheWithNoStore_ThenEmitNoCacheConvention() {
        var source = GeneratorTestHelper.GetGeneratedSource(OutputCacheWithNoStoreEndpoint);

        Assert.Contains($"{SourceCodeHelper.Namespace}.OutputCacheWithNoStoreEndpoint", source);
        Assert.Matches(@"OutputCacheWithNoStoreEndpoint\.Register\(.*\);", source);
        Assert.Matches(@"OutputCacheWithNoStoreEndpoint\.Map\(.*\);", source);
        Assert.Matches(@"\.CacheOutput\(policy => policy\.NoCache\(\)\)", source);
    }

    [Fact]
    public void WhenEndpointClassMarkOutputCacheWithPolicyName_ThenEmitNamedPolicyConvention() {
        var source = GeneratorTestHelper.GetGeneratedSource(OutputCacheWithPolicyNameEndpoint);

        Assert.Contains($"{SourceCodeHelper.Namespace}.OutputCacheWithPolicyNameEndpoint", source);
        Assert.Matches(@"OutputCacheWithPolicyNameEndpoint\.Register\(.*\);", source);
        Assert.Matches(@"OutputCacheWithPolicyNameEndpoint\.Map\(.*\);", source);
        Assert.Matches(@"\.CacheOutput\(""my-cache-policy""\)", source);
    }

    [Fact]
    public void WhenEndpointClassMarkOutputCacheWithDuration_ThenEmitExpireConvention() {
        var source = GeneratorTestHelper.GetGeneratedSource(OutputCacheWithDurationEndpoint);

        Assert.Contains($"{SourceCodeHelper.Namespace}.OutputCacheWithDurationEndpoint", source);
        Assert.Matches(@"OutputCacheWithDurationEndpoint\.Register\(.*\);", source);
        Assert.Matches(@"OutputCacheWithDurationEndpoint\.Map\(.*\);", source);
        Assert.Contains(".CacheOutput(policy => {", source);
        Assert.Contains("policy.Cache();", source);
        Assert.Contains("policy.Expire(TimeSpan.FromSeconds(60));", source);
    }

    [Fact]
    public void WhenEndpointClassMarkOutputCacheWithVaryByQueryKeys_ThenEmitVaryByQueryConvention() {
        var source = GeneratorTestHelper.GetGeneratedSource(OutputCacheWithVaryByQueryKeysEndpoint);

        Assert.Contains($"{SourceCodeHelper.Namespace}.OutputCacheWithVaryByQueryKeysEndpoint", source);
        Assert.Matches(@"OutputCacheWithVaryByQueryKeysEndpoint\.Register\(.*\);", source);
        Assert.Matches(@"OutputCacheWithVaryByQueryKeysEndpoint\.Map\(.*\);", source);
        Assert.Contains(".CacheOutput(policy => {", source);
        Assert.Contains(@"policy.SetVaryByQuery(""search"", ""page"");", source);
    }

    [Fact]
    public void WhenEndpointClassMarkOutputCacheWithTags_ThenEmitTagConvention() {
        var source = GeneratorTestHelper.GetGeneratedSource(OutputCacheWithTagsEndpoint);

        Assert.Contains($"{SourceCodeHelper.Namespace}.OutputCacheWithTagsEndpoint", source);
        Assert.Matches(@"OutputCacheWithTagsEndpoint\.Register\(.*\);", source);
        Assert.Matches(@"OutputCacheWithTagsEndpoint\.Map\(.*\);", source);
        Assert.Contains(".CacheOutput(policy => {", source);
        Assert.Contains(@"policy.Tag(""products"");", source);
    }
}
