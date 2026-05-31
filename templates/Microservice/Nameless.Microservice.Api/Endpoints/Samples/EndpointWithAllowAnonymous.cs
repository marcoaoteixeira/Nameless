using Nameless.Web.Http.Endpoints;
using Nameless.Web.Http.Endpoints.Attributes;
using Nameless.Web.Http.Endpoints.Attributes.Authorization;

namespace Nameless.Microservice.Api.Endpoints.Samples;

[Endpoint<Get>("/AllowAnonymous", Name = "EndpointWithAllowAnonymous", Tags = ["AllowAnonymous"])]
[AllowAnonymous]
public partial class EndpointWithAllowAnonymous   {
    public Task<IResult> HandleAsync() { 
        return Task.FromResult<IResult>(TypedResults.Ok(new { Message = "Allow Anonymous Sample" }));
    }
}