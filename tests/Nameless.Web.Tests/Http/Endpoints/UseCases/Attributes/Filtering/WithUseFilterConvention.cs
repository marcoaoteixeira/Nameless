using Nameless.Testing.Tools.Attributes;
using Nameless.Web.Http.Endpoints.Infrastructure;

namespace Nameless.Web.Http.Endpoints.UseCases.Attributes.Filtering;

[UnitTest]
public class WithUseFilterConvention {
    private static string UseFilterWithGenericSyntaxEndpoint => SourceCodeHelper.Write(
        """
        public class MyEndpointFilter : IEndpointFilter {
            public ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
                => next(context);
        }

        [Endpoint]
        [UseFilter<MyEndpointFilter>]
        public partial class UseFilterWithGenericSyntaxEndpoint {
            public Task<IResult> HandleAsync() {
                return Task.FromResult<IResult>(TypedResults.Ok());
            }
        }
        """
    );

    private static string UseFilterWithTypeofSyntaxEndpoint => SourceCodeHelper.Write(
        """
        public class AnotherEndpointFilter : IEndpointFilter {
            public ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
                => next(context);
        }

        [Endpoint]
        [UseFilter(typeof(AnotherEndpointFilter))]
        public partial class UseFilterWithTypeofSyntaxEndpoint {
            public Task<IResult> HandleAsync() {
                return Task.FromResult<IResult>(TypedResults.Ok());
            }
        }
        """
    );

    [Fact]
    public void WhenEndpointClassMarkUseFilterWithGenericSyntax_ThenEmitConvention() {
        var source = GeneratorTestHelper.GetGeneratedSource(UseFilterWithGenericSyntaxEndpoint);

        Assert.Contains($"{SourceCodeHelper.Namespace}.UseFilterWithGenericSyntaxEndpoint", source);
        Assert.Matches(@"UseFilterWithGenericSyntaxEndpoint\.Register\(.*\);", source);
        Assert.Matches(@"UseFilterWithGenericSyntaxEndpoint\.Map\(.*\);", source);
        Assert.Matches(@"\.AddEndpointFilter<global::Nameless\.Web\.Http\.Endpoint\.TestUseCases\.MyEndpointFilter>\(\)", source);
    }

    [Fact]
    public void WhenEndpointClassMarkUseFilterWithTypeofSyntax_ThenEmitConvention() {
        var source = GeneratorTestHelper.GetGeneratedSource(UseFilterWithTypeofSyntaxEndpoint);

        Assert.Contains($"{SourceCodeHelper.Namespace}.UseFilterWithTypeofSyntaxEndpoint", source);
        Assert.Matches(@"UseFilterWithTypeofSyntaxEndpoint\.Register\(.*\);", source);
        Assert.Matches(@"UseFilterWithTypeofSyntaxEndpoint\.Map\(.*\);", source);
        Assert.Matches(@"\.AddEndpointFilter<global::Nameless\.Web\.Http\.Endpoint\.TestUseCases\.AnotherEndpointFilter>\(\)", source);
    }
}
