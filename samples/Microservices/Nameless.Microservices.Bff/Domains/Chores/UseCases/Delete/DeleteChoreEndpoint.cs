using System.Net;
using Microsoft.AspNetCore.Mvc;
using Nameless.Microservices.Bff.Domains.Chores.External;
using Nameless.ObjectModel;
using Nameless.Web.Http.Endpoints;
using Nameless.Web.Http.Endpoints.Attributes.Produces;
using Refit;

namespace Nameless.Microservices.Bff.Domains.Chores.UseCases.Delete;

[Endpoint(
    route: "/{id:guid}",
    Verb = HttpVerbs.Delete,
    Description = "Deletes a chore from your list. Dodging responsibilities already?? 😒",
    Group = typeof(ChoresEndpointGroup)
)]
[ProducesResponse<Nothing>(StatusCode = StatusCodes.Status204NoContent)]
[ProducesProblemResponse]
[ProducesProblemResponse(StatusCode = StatusCodes.Status404NotFound)]
public partial class DeleteChoreEndpoint {
    private readonly IChoresHttpClient _httpClient;

    public DeleteChoreEndpoint(IChoresHttpClient httpClient) {
        _httpClient = httpClient;
    }

    public async Task<IResult> HandleAsync([FromRoute] Guid id, CancellationToken cancellationToken) {
        try {
            await _httpClient.DeleteChoreAsync(id, cancellationToken).ConfigureAwait(false);
        }
        catch (ApiException ex) when (ex.StatusCode == HttpStatusCode.NotFound) {
            return TypedResults.NotFound();
        }
        catch (ApiException ex) {
            return TypedResults.StatusCode((int)ex.StatusCode);
        }

        return TypedResults.NoContent();
    }
}
