using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Nameless.Web.Http.Endpoints;
using Nameless.Web.Http.Endpoints.Attributes;

namespace Nameless.Microservice.Api.Endpoints.Samples;

[Endpoint<Get>("/AllowAnonymous", Name = "EndpointWithAllowAnonymous", Tags = ["AllowAnonymous"])]
[AllowAnonymous]
public partial class EndpointWithAllowAnonymous {
    public Task<IResult> HandleAsync() {
        return Task.FromResult<IResult>(TypedResults.Ok(new { Message = "Allow Anonymous Sample" }));
    }
}