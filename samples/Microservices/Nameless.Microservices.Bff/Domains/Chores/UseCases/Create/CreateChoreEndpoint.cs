using System.Net;
using Microsoft.AspNetCore.Mvc;
using Nameless.Microservices.Bff.Domains.Chores.External;
using Nameless.Microservices.Common.Chores.Inputs;
using Nameless.Microservices.Common.Chores.Outputs;
using Nameless.Web.Http.Endpoints;
using Nameless.Web.Http.Endpoints.Attributes.Produces;
using Refit;

namespace Nameless.Microservices.Bff.Domains.Chores.UseCases.Create;

[Endpoint(
    Verb = HttpVerbs.Post,
    Name = "Create Chore",
    Description = "Creates a new chore in your daily list. Time to make Future You proud. 😁",
    Group = typeof(ChoresEndpointGroup)
)]
[ProducesResponse<CreateChoreOutput>]
[ProducesProblemResponse]
[ProducesValidationProblemResponse]
public partial class CreateChoreEndpoint {
    private readonly IChoresHttpClient _httpClient;

    public CreateChoreEndpoint(IChoresHttpClient httpClient) {
        _httpClient = httpClient;
    }

    public async Task<IResult> HandleAsync(CreateChoreInput input, CancellationToken cancellationToken) {
        try {
            var response = await _httpClient
                .CreateChoreAsync(input, cancellationToken)
                .ConfigureAwait(false);

            return TypedResults.Ok(response);
        }
        catch (ApiException ex) when (ex.StatusCode == HttpStatusCode.BadRequest) {
            var problem = await ex.GetContentAsAsync<ValidationProblemDetails>();
            return problem is not null
                ? TypedResults.ValidationProblem(problem.Errors)
                : TypedResults.BadRequest();
        }
        catch (ApiException ex) {
            return TypedResults.StatusCode((int)ex.StatusCode);
        }
    }
}
