using Nameless.Testing.Tools.Attributes;
using Nameless.Web.Http.Endpoints.Infrastructure;

namespace Nameless.Web.Http.Endpoints.UseCases.Attributes.Versioning;

[UnitTest]
public class WithDeprecateConvention {
    private static string DeprecateWithNoArgumentsEndpoint => SourceCodeHelper.Write(
        """
        [Endpoint<Get>]
        [Deprecate]
        public partial class DeprecateWithNoArgumentsEndpoint {
            public Task<IResult> HandleAsync() {
                return Task.FromResult<IResult>(TypedResults.Ok());
            }
        }
        """
    );

    private static string DeprecateWithMessageEndpoint => SourceCodeHelper.Write(
        """
        [Endpoint<Get>]
        [Deprecate(Message = "Use /v2/items instead.")]
        public partial class DeprecateWithMessageEndpoint {
            public Task<IResult> HandleAsync() {
                return Task.FromResult<IResult>(TypedResults.Ok());
            }
        }
        """
    );

    private static string DeprecateWithSunsetEndpoint => SourceCodeHelper.Write(
        """
        [Endpoint<Get>]
        [Deprecate(Sunset = "Wed, 31 Dec 2025 00:00:00 GMT")]
        public partial class DeprecateWithSunsetEndpoint {
            public Task<IResult> HandleAsync() {
                return Task.FromResult<IResult>(TypedResults.Ok());
            }
        }
        """
    );

    private static string DeprecateWithSunsetAndLinkEndpoint => SourceCodeHelper.Write(
        """
        [Endpoint<Get>]
        [Deprecate(Sunset = "Wed, 31 Dec 2025 00:00:00 GMT", Link = "https://example.com/migration")]
        public partial class DeprecateWithSunsetAndLinkEndpoint {
            public Task<IResult> HandleAsync() {
                return Task.FromResult<IResult>(TypedResults.Ok());
            }
        }
        """
    );

    [Fact]
    public void WhenEndpointClassMarkDeprecateWithNoArguments_ThenEmitConvention() {
        var source = GeneratorTestHelper.GetGeneratedSource(DeprecateWithNoArgumentsEndpoint);

        Assert.Contains($"{SourceCodeHelper.Namespace}.DeprecateWithNoArgumentsEndpoint", source);
        Assert.Matches(@"DeprecateWithNoArgumentsEndpoint\.Register\(.*\);", source);
        Assert.Matches(@"DeprecateWithNoArgumentsEndpoint\.Map\(.*\);", source);
        Assert.Matches(@"\.AddOpenApiOperationTransformer\(", source);
        Assert.Contains("op.Deprecated = true;", source);
    }

    [Fact]
    public void WhenEndpointClassMarkDeprecateWithMessage_ThenEmitConventionWithDescription() {
        var source = GeneratorTestHelper.GetGeneratedSource(DeprecateWithMessageEndpoint);

        Assert.Contains($"{SourceCodeHelper.Namespace}.DeprecateWithMessageEndpoint", source);
        Assert.Matches(@"DeprecateWithMessageEndpoint\.Register\(.*\);", source);
        Assert.Matches(@"DeprecateWithMessageEndpoint\.Map\(.*\);", source);
        Assert.Contains("op.Deprecated = true;", source);
        Assert.Contains(@"op.Description = ""Use /v2/items instead."";", source);
    }

    [Fact]
    public void WhenEndpointClassMarkDeprecateWithSunset_ThenEmitConventionWithSunsetCall() {
        var source = GeneratorTestHelper.GetGeneratedSource(DeprecateWithSunsetEndpoint);

        Assert.Contains($"{SourceCodeHelper.Namespace}.DeprecateWithSunsetEndpoint", source);
        Assert.Matches(@"DeprecateWithSunsetEndpoint\.Register\(.*\);", source);
        Assert.Matches(@"DeprecateWithSunsetEndpoint\.Map\(.*\);", source);
        Assert.Contains("op.Deprecated = true;", source);
        Assert.Matches(@"\.WithSunset\(DateTimeOffset\.ParseExact\(""Wed, 31 Dec 2025 00:00:00 GMT"", ""R"", CultureInfo\.InvariantCulture\)\)", source);
    }

    [Fact]
    public void WhenEndpointClassMarkDeprecateWithSunsetAndLink_ThenEmitConventionWithSunsetAndLinkCall() {
        var source = GeneratorTestHelper.GetGeneratedSource(DeprecateWithSunsetAndLinkEndpoint);

        Assert.Contains($"{SourceCodeHelper.Namespace}.DeprecateWithSunsetAndLinkEndpoint", source);
        Assert.Matches(@"DeprecateWithSunsetAndLinkEndpoint\.Register\(.*\);", source);
        Assert.Matches(@"DeprecateWithSunsetAndLinkEndpoint\.Map\(.*\);", source);
        Assert.Contains("op.Deprecated = true;", source);
        Assert.Matches(@"\.WithSunset\(DateTimeOffset\.ParseExact\(""Wed, 31 Dec 2025 00:00:00 GMT"", ""R"", CultureInfo\.InvariantCulture\), link: ""https://example\.com/migration""\)", source);
    }
}
