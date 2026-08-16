using Nameless.Testing.Tools.Attributes;

namespace Nameless.Generators.Web.Http.Endpoints.Attributes.Filtering;

[UnitTest]
public class UseFilterAttributeUsageTests
{
    [Fact]
    public void WhenGenerateWithGenericTypeArgument_ThenAppliesCorrectConvention()
    {
        var code = SourceCodeHelper.Write(
            """
            public class SampleFilter : IEndpointFilter {
                public ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
                    => next(context);
            }

            [Endpoint]
            [UseFilter<SampleFilter>]
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
        Assert.Matches(@"\.AddEndpointFilter<.*SampleFilter>\(\)", source);
    }

    [Fact]
    public void WhenGenerateWithConstructorTypeArgument_ThenAppliesCorrectConvention()
    {
        var code = SourceCodeHelper.Write(
            """
            public class SampleFilter : IEndpointFilter {
                public ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
                    => next(context);
            }

            [Endpoint]
            [UseFilter(typeof(SampleFilter))]
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
        Assert.Matches(@"\.AddEndpointFilter<.*SampleFilter>\(\)", source);
    }
}
