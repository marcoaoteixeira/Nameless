using Nameless.Web.Http.Endpoints.Infrastructure;

namespace Nameless.Web.Http.Endpoints.Cases;

public sealed class ProducesMetadataTests
{
    [Fact]
    public void Generate_WhenProducesAttribute_ThenEmitsProducesCall()
    {
        const string source = """
            using Nameless.Web.Http.Endpoints.Attributes;
            using Microsoft.AspNetCore.Http;
            using System.Threading.Tasks;

            namespace TestApp;

            [Endpoint<Get>("/users")]
            [Produces<string>(200)]
            public class GetUsersEndpoint
            {
                public async Task<IResult> HandleAsync() => Results.Ok();
            }
            """;

        var generated = GeneratorTestHelper.GetGeneratedSource(source);

        Assert.Contains(".Produces<", generated);
        Assert.Contains("200", generated);
    }

    [Fact]
    public void Generate_WhenProducesProblemAttribute_ThenEmitsProducesProblem()
    {
        const string source = """
            using Nameless.Web.Http.Endpoints.Attributes;
            using Microsoft.AspNetCore.Http;
            using System.Threading.Tasks;

            namespace TestApp;

            [Endpoint<Get>("/users")]
            [ProducesProblem(500)]
            public class GetUsersEndpoint
            {
                public async Task<IResult> HandleAsync() => Results.Ok();
            }
            """;

        var generated = GeneratorTestHelper.GetGeneratedSource(source);

        Assert.Contains(".ProducesProblem(statusCode: 500", generated);
    }

    [Fact]
    public void Generate_WhenProducesValidationProblemAttribute_ThenEmitsProducesValidationProblem()
    {
        const string source = """
            using Nameless.Web.Http.Endpoints.Attributes;
            using Microsoft.AspNetCore.Http;
            using System.Threading.Tasks;

            namespace TestApp;

            [Endpoint<Post>("/users")]
            [ProducesValidationProblem(400)]
            public class CreateUserEndpoint
            {
                public async Task<IResult> HandleAsync() => Results.Ok();
            }
            """;

        var generated = GeneratorTestHelper.GetGeneratedSource(source);

        Assert.Contains(".ProducesValidationProblem(statusCode: 400", generated);
    }

    [Fact]
    public void Generate_WhenMultipleProducesAttributes_ThenAllEmitted()
    {
        const string source = """
            using Nameless.Web.Http.Endpoints.Attributes;
            using Microsoft.AspNetCore.Http;
            using System.Threading.Tasks;

            namespace TestApp;

            [Endpoint<Get>("/users")]
            [Produces<string>(200)]
            [ProducesProblem(500)]
            [ProducesValidationProblem(400)]
            public class GetUsersEndpoint
            {
                public async Task<IResult> HandleAsync() => Results.Ok();
            }
            """;

        var generated = GeneratorTestHelper.GetGeneratedSource(source);

        Assert.Contains(".Produces<", generated);
        Assert.Contains(".ProducesProblem(statusCode: 500", generated);
        Assert.Contains(".ProducesValidationProblem(statusCode: 400", generated);
    }
}
