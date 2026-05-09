using Nameless.Web.Http.Endpoints.Infrastructure;

namespace Nameless.Web.Http.Endpoints.Cases;

public sealed class MultipleFiltersTests
{
    private static string BuildSourceWithFilters(string filterAttributes) {
        return $$"""
                 using Nameless.Web.Http.Endpoints.Attributes;
                 using Microsoft.AspNetCore.Http;
                 using System.Threading;
                 using System.Threading.Tasks;
                 using System.ValueTuple;

                 namespace TestApp;

                 public class LoggingFilter : IEndpointFilter
                 {
                     public ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext ctx, EndpointFilterDelegate next)
                         => next(ctx);
                 }

                 public class AuthFilter : IEndpointFilter
                 {
                     public ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext ctx, EndpointFilterDelegate next)
                         => next(ctx);
                 }

                 [Endpoint<Get>("/users")]
                 {{filterAttributes}}
                 public class GetUsersEndpoint
                 {
                     public async Task<IResult> HandleAsync() => Results.Ok();
                 }
                 """;
    }

    [Fact]
    public void Generate_WhenSingleFilterAttribute_ThenEmitsAddEndpointFilter()
    {
        var source = BuildSourceWithFilters("[Filter<LoggingFilter>]");

        var generated = GeneratorTestHelper.GetGeneratedSource(source);

        Assert.Contains(".AddEndpointFilter<global::TestApp.LoggingFilter>()", generated);
    }

    [Fact]
    public void Generate_WhenMultipleFilterAttributes_ThenAllFiltersChained()
    {
        var source = BuildSourceWithFilters("[Filter<LoggingFilter>]\n[Filter<AuthFilter>]");

        var generated = GeneratorTestHelper.GetGeneratedSource(source);

        Assert.Contains(".AddEndpointFilter<global::TestApp.LoggingFilter>()", generated);
        Assert.Contains(".AddEndpointFilter<global::TestApp.AuthFilter>()", generated);
    }
}
