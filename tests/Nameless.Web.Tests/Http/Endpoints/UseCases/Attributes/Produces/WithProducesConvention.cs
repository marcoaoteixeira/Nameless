using Nameless.Testing.Tools.Attributes;
using Nameless.Web.Http.Endpoints.Infrastructure;

namespace Nameless.Web.Http.Endpoints.UseCases.Attributes.Produces;

[UnitTest]
public class WithProducesConvention {
    private static string ProducesWithGenericSyntaxDefaultsEndpoint => SourceCodeHelper.Write(
        """
        [Endpoint]
        [ProducesResponse<string>]
        public partial class ProducesWithGenericSyntaxDefaultsEndpoint {
            public Task<IResult> HandleAsync() {
                return Task.FromResult<IResult>(TypedResults.Ok("OK"));
            }
        }
        """
    );

    private static string ProducesWithGenericSyntaxCustomStatusCodeEndpoint => SourceCodeHelper.Write(
        """
        [Endpoint(Verb = HttpVerbs.Post)]
        [ProducesResponse<string>(StatusCode = 201)]
        public partial class ProducesWithGenericSyntaxCustomStatusCodeEndpoint {
            public Task<IResult> HandleAsync() {
                return Task.FromResult<IResult>(TypedResults.Created());
            }
        }
        """
    );

    private static string ProducesWithGenericSyntaxCustomContentTypeEndpoint => SourceCodeHelper.Write(
        """
        [Endpoint(Verb = HttpVerbs.Post)]
        [ProducesResponse<string>(ContentType = "text/plain")]
        public partial class ProducesWithGenericSyntaxCustomContentTypeEndpoint {
            public Task<IResult> HandleAsync() {
                return Task.FromResult<IResult>(TypedResults.Created());
            }
        }
        """
    );

    private static string ProducesWithGenericSyntaxAdditionalContentTypesEndpoint => SourceCodeHelper.Write(
        """
        [Endpoint(Verb = HttpVerbs.Post)]
        [ProducesResponse<string>(AdditionalContentTypes = ["text/plain", "text/csv"])]
        public partial class ProducesWithGenericSyntaxAdditionalContentTypesEndpoint {
            public Task<IResult> HandleAsync() {
                return Task.FromResult<IResult>(TypedResults.Created());
            }
        }
        """
    );

    private static string ProducesWithTypeofSyntaxEndpoint => SourceCodeHelper.Write(
        """
        [Endpoint]
        [ProducesResponse(typeof(string))]
        public partial class ProducesWithTypeofSyntaxEndpoint {
            public Task<IResult> HandleAsync() {
                return Task.FromResult<IResult>(TypedResults.Ok("OK"));
            }
        }
        """
    );

    private static string ProducesWithTypeofSyntaxCustomStatusCodeEndpoint => SourceCodeHelper.Write(
        """
        [Endpoint(Verb = HttpVerbs.Post)]
        [ProducesResponse(typeof(string), StatusCode = 201)]
        public partial class ProducesWithTypeofSyntaxCustomStatusCodeEndpoint {
            public Task<IResult> HandleAsync() {
                return Task.FromResult<IResult>(TypedResults.Created());
            }
        }
        """
    );

    private static string ProducesWithTypeofSyntaxCustomContentTypeEndpoint => SourceCodeHelper.Write(
        """
        [Endpoint(Verb = HttpVerbs.Post)]
        [ProducesResponse(typeof(string), ContentType = "text/plain")]
        public partial class ProducesWithTypeofSyntaxCustomContentTypeEndpoint {
            public Task<IResult> HandleAsync() {
                return Task.FromResult<IResult>(TypedResults.Created());
            }
        }
        """
    );

    private static string ProducesWithTypeofSyntaxAdditionalContentTypesEndpoint => SourceCodeHelper.Write(
        """
        [Endpoint(Verb = HttpVerbs.Post)]
        [ProducesResponse(typeof(string), AdditionalContentTypes = ["text/plain", "text/csv"])]
        public partial class ProducesWithTypeofSyntaxAdditionalContentTypesEndpoint {
            public Task<IResult> HandleAsync() {
                return Task.FromResult<IResult>(TypedResults.Created());
            }
        }
        """
    );

    [Fact]
    public void WhenEndpointClassMarkProducesWithGenericSyntaxDefaults_ThenEmitConvention() {
        var source = GeneratorTestHelper.GetGeneratedSource(ProducesWithGenericSyntaxDefaultsEndpoint);

        Assert.Contains($"{SourceCodeHelper.Namespace}.ProducesWithGenericSyntaxDefaultsEndpoint", source);
        Assert.Matches(@"ProducesWithGenericSyntaxDefaultsEndpoint\.Register\(.*\);", source);
        Assert.Matches(@"ProducesWithGenericSyntaxDefaultsEndpoint\.Map\(.*\);", source);
        Assert.Matches(@"\.Produces<string>\(statusCode: 200, contentType: ""application/json"", additionalContentTypes: \[\]\)", source);
    }

    [Fact]
    public void WhenEndpointClassMarkProducesWithGenericSyntaxCustomStatusCode_ThenEmitConvention() {
        var source = GeneratorTestHelper.GetGeneratedSource(ProducesWithGenericSyntaxCustomStatusCodeEndpoint);

        Assert.Contains($"{SourceCodeHelper.Namespace}.ProducesWithGenericSyntaxCustomStatusCodeEndpoint", source);
        Assert.Matches(@"ProducesWithGenericSyntaxCustomStatusCodeEndpoint\.Register\(.*\);", source);
        Assert.Matches(@"ProducesWithGenericSyntaxCustomStatusCodeEndpoint\.Map\(.*\);", source);
        Assert.Matches(@"\.Produces<string>\(statusCode: 201, contentType: ""application/json"", additionalContentTypes: \[\]\)", source);
    }

    [Fact]
    public void WhenEndpointClassMarkProducesWithGenericSyntaxCustomContentType_ThenEmitConvention() {
        var source = GeneratorTestHelper.GetGeneratedSource(ProducesWithGenericSyntaxCustomContentTypeEndpoint);

        Assert.Contains($"{SourceCodeHelper.Namespace}.ProducesWithGenericSyntaxCustomContentTypeEndpoint", source);
        Assert.Matches(@"ProducesWithGenericSyntaxCustomContentTypeEndpoint\.Register\(.*\);", source);
        Assert.Matches(@"ProducesWithGenericSyntaxCustomContentTypeEndpoint\.Map\(.*\);", source);
        Assert.Matches(@"\.Produces<string>\(statusCode: 200, contentType: ""text/plain"", additionalContentTypes: \[\]\)", source);
    }

    [Fact]
    public void WhenEndpointClassMarkProducesWithGenericSyntaxAdditionalContentTypes_ThenEmitConvention() {
        var source = GeneratorTestHelper.GetGeneratedSource(ProducesWithGenericSyntaxAdditionalContentTypesEndpoint);

        Assert.Contains($"{SourceCodeHelper.Namespace}.ProducesWithGenericSyntaxAdditionalContentTypesEndpoint", source);
        Assert.Matches(@"ProducesWithGenericSyntaxAdditionalContentTypesEndpoint\.Register\(.*\);", source);
        Assert.Matches(@"ProducesWithGenericSyntaxAdditionalContentTypesEndpoint\.Map\(.*\);", source);
        Assert.Matches(@"\.Produces<string>\(statusCode: 200, contentType: ""application/json"", additionalContentTypes: \[""text/plain"", ""text/csv""\]\)", source);
    }

    [Fact]
    public void WhenEndpointClassMarkProducesWithTypeofSyntax_ThenEmitConvention() {
        var source = GeneratorTestHelper.GetGeneratedSource(ProducesWithTypeofSyntaxEndpoint);

        Assert.Contains($"{SourceCodeHelper.Namespace}.ProducesWithTypeofSyntaxEndpoint", source);
        Assert.Matches(@"ProducesWithTypeofSyntaxEndpoint\.Register\(.*\);", source);
        Assert.Matches(@"ProducesWithTypeofSyntaxEndpoint\.Map\(.*\);", source);
        Assert.Matches(@"\.Produces\(responseType: typeof\(string\), statusCode: 200, contentType: ""application/json"", additionalContentTypes: \[\]\)", source);
    }

    [Fact]
    public void WhenEndpointClassMarkProducesWithTypeofSyntaxCustomStatusCode_ThenEmitConvention() {
        var source = GeneratorTestHelper.GetGeneratedSource(ProducesWithTypeofSyntaxCustomStatusCodeEndpoint);

        Assert.Contains($"{SourceCodeHelper.Namespace}.ProducesWithTypeofSyntaxCustomStatusCodeEndpoint", source);
        Assert.Matches(@"ProducesWithTypeofSyntaxCustomStatusCodeEndpoint\.Register\(.*\);", source);
        Assert.Matches(@"ProducesWithTypeofSyntaxCustomStatusCodeEndpoint\.Map\(.*\);", source);
        Assert.Matches(@"\.Produces\(responseType: typeof\(string\), statusCode: 201, contentType: ""application/json"", additionalContentTypes: \[\]\)", source);
    }

    [Fact]
    public void WhenEndpointClassMarkProducesWithTypeofSyntaxCustomContentType_ThenEmitConvention() {
        var source = GeneratorTestHelper.GetGeneratedSource(ProducesWithTypeofSyntaxCustomContentTypeEndpoint);

        Assert.Contains($"{SourceCodeHelper.Namespace}.ProducesWithTypeofSyntaxCustomContentTypeEndpoint", source);
        Assert.Matches(@"ProducesWithTypeofSyntaxCustomContentTypeEndpoint\.Register\(.*\);", source);
        Assert.Matches(@"ProducesWithTypeofSyntaxCustomContentTypeEndpoint\.Map\(.*\);", source);
        Assert.Matches(@"\.Produces\(responseType: typeof\(string\), statusCode: 200, contentType: ""text/plain"", additionalContentTypes: \[\]\)", source);
    }

    [Fact]
    public void WhenEndpointClassMarkProducesWithTypeofSyntaxAdditionalContentTypes_ThenEmitConvention() {
        var source = GeneratorTestHelper.GetGeneratedSource(ProducesWithTypeofSyntaxAdditionalContentTypesEndpoint);

        Assert.Contains($"{SourceCodeHelper.Namespace}.ProducesWithTypeofSyntaxAdditionalContentTypesEndpoint", source);
        Assert.Matches(@"ProducesWithTypeofSyntaxAdditionalContentTypesEndpoint\.Register\(.*\);", source);
        Assert.Matches(@"ProducesWithTypeofSyntaxAdditionalContentTypesEndpoint\.Map\(.*\);", source);
        Assert.Matches(@"\.Produces\(responseType: typeof\(string\), statusCode: 200, contentType: ""application/json"", additionalContentTypes: \[""text/plain"", ""text/csv""\]\)", source);
    }
}
