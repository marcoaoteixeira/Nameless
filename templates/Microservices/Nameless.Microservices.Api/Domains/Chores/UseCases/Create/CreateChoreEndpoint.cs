using Nameless.Web.Http.Endpoints;

namespace Nameless.Microservices.Api.Domains.Chores.UseCases.Create;

[Endpoint<Post>(Group = typeof(ChoresEndpointGroup))]
public partial class CreateChoreEndpoint {
    public Task<IResult> HandleAsync() {
        return Task.FromResult<IResult>(TypedResults.Ok());
    }
}
