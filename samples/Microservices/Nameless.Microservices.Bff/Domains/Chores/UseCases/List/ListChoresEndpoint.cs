using Nameless.Microservices.Bff.Domains.Chores.External;
using Nameless.Microservices.Common.Chores.DTOs;
using Nameless.Microservices.Common.Chores.Inputs;
using Nameless.Web.Http.Endpoints;
using Nameless.Web.Http.Endpoints.Attributes.Produces;

namespace Nameless.Microservices.Bff.Domains.Chores.UseCases.List;

[Endpoint(
    Description = "Lists all your daily chores. Let's see what today's version of you promised yesterday's version. 🥲",
    Group = typeof(ChoresEndpointGroup)
)]
[ProducesResponse<ChoreDto[]>]
public partial class ListChoresEndpoint {
    private readonly IChoresHttpClient _httpClient;

    public ListChoresEndpoint(IChoresHttpClient httpClient) {
        _httpClient = httpClient;
    }

    public async Task<IResult> HandleAsync([AsParameters] ListChoresInput input, CancellationToken cancellationToken) {
        var chores = await _httpClient
            .ListChoresAsync(input, cancellationToken)
            .ConfigureAwait(false);

        return TypedResults.Ok(chores);
    }
}
