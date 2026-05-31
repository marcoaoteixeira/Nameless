using Nameless.Web.Http.Endpoints.Infrastructure;

namespace Nameless.Web.Http.Endpoints.Cases;

public class UsingAllAttributesTests {
    [Fact]
    public void Generate_WhenProducesAttribute_ThenEmitsProducesCall() {
        const string source = """
                              using global::Nameless.Web.Http.Endpoints.Attributes;
                              using global::Nameless.Web.Http.Endpoints.Attributes.Antiforgery;
                              using global::Nameless.Web.Http.Endpoints.Attributes.Authorization;
                              using global::Nameless.Web.Http.Endpoints.Attributes.CookieRedirect;
                              using global::Nameless.Web.Http.Endpoints.Attributes.Filtering;
                              using global::Nameless.Web.Http.Endpoints.Attributes.HttpMetrics;
                              using global::Nameless.Web.Http.Endpoints.Attributes.OutputCache;
                              using global::Nameless.Web.Http.Endpoints.Attributes.Produces;
                              using global::Nameless.Web.Http.Endpoints.Attributes.RateLimiting;
                              using global::Nameless.Web.Http.Endpoints.Attributes.RequestTimeout;
                              using global::Nameless.Web.Http.Endpoints.Attributes.Validation;
                              using global::Nameless.Web.Http.Endpoints.Attributes.Versioning;
                              using Microsoft.AspNetCore.Http;
                              using Nameless.Web.Http.Endpoints;
                              
                              using AllowCookieRedirectAttribute = global::Nameless.Web.Http.Endpoints.Attributes.CookieRedirect.AllowCookieRedirectAttribute;
                              using DisableHttpMetricsAttribute = global::Nameless.Web.Http.Endpoints.Attributes.HttpMetrics.DisableHttpMetricsAttribute;
                              using DeprecateAttribute = global::Nameless.Web.Http.Endpoints.Attributes.Versioning.DeprecateAttribute;
                              
                              namespace TestApp;
                              
                              internal class GenericEndpointFilter : IEndpointFilter {
                                  public ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next) {
                                      throw new NotImplementedException();
                                  }
                              }
                              
                              [EndpointGroup("MyGroup", "/api/v{version:apiVersion}/myeps")]
                              public partial class MyEndpointGroup;
                              
                              [Endpoint<Get>("/users", Name = "Test", Description = "Test", Summary = "Test", Tags = ["Test"], Version = "1.2-alpha", Group = typeof(MyEndpointGroup))]
                              [DisableAntiforgery]
                              [AllowAnonymous]
                              [UseAuthorization]
                              [UseAuthorization(PolicyName = "policy-name")]
                              [UseAuthorization(PolicyName = "policy-name", Roles = "A, B", AuthenticationSchemes = "C, D")]
                              [AllowCookieRedirect]
                              [DisableCookieRedirect]
                              [UseFilter<GenericEndpointFilter>]
                              [UseFilter(filterType: typeof(GenericEndpointFilter))]
                              [DisableHttpMetrics]
                              [DisableOutputCache]
                              [UseOutputCache("policy-name")]
                              [Produces<string>]
                              [Produces<int>(statusCode: 201, contentType: "application/json")]
                              [Produces(responseType: typeof(double), statusCode: 202, contentType: "application/json")]
                              [ProducesProblem]
                              [ProducesProblem(statusCode: 501)]
                              [ProducesProblem(statusCode: 502, contentType: "application/json")]
                              [ProducesValidationProblem(statusCode: 402, contentType: "application/json")]
                              [DisableRateLimiting]
                              [UseRateLimiting("policy-name")]
                              [DisableRequestTimeout]
                              [UseRequestTimeout("policy-name")]
                              [DisableValidation]
                              [Deprecate(Message = "Use something else instead", Sunset = "Wed, 11 Nov 2026 11:11:11 GMT")]
                              public partial class AllAttributesEndpoint {
                                  public Task<IResult> HandleAsync() {
                                      return Task.FromResult(Results.Ok());
                                  }
                              }
                              """;

        var generated = GeneratorTestHelper.GetGeneratedSource(source);

        Assert.Contains(".Produces<", generated);
        Assert.Contains("200", generated);
    }
}
