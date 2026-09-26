using System.Net;
using Microsoft.AspNetCore.Mvc;
using Nameless.Microservices.Bff.Domains.Chores.External;
using Nameless.ObjectModel;
using Nameless.Web.Http.Endpoints;
using Nameless.Web.Http.Endpoints.Attributes.Produces;
using Refit;

namespace Nameless.Microservices.Bff.Domains.Chores.UseCases.MarkDone;

[Endpoint(
    route: "/{id:guid}",
    Verb = HttpVerbs.Post,
    Description = "Marks the chore as done. Go ahead, you've earned a cookie. 🍪",
    Group = typeof(ChoresEndpointGroup)
)]
[ProducesResponse<Nothing>(StatusCode = StatusCodes.Status204NoContent)]
[ProducesProblemResponse]
[ProducesProblemResponse(StatusCode = StatusCodes.Status404NotFound)]
public partial class MarkDoneEndpoint {
    private readonly IChoresHttpClient _httpClient;

    public MarkDoneEndpoint(IChoresHttpClient httpClient) {
        _httpClient = httpClient;
    }

    public async Task<IResult> HandleAsync([FromRoute] Guid id, CancellationToken cancellationToken) {
        try {
            await _httpClient.MarkDoneAsync(id, cancellationToken).ConfigureAwait(false);
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
