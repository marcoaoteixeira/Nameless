using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.OutputCaching;
using Nameless.Web.Http.Endpoints;
using Nameless.Web.Http.Endpoints.Attributes;
using Nameless.Web.Http.Endpoints.Attributes.Antiforgery;

namespace Nameless.Microservice.Api.Endpoints.Samples;

[Endpoint<Get>("/DisableAntiforgery", Name = "EndpointWithDisableAntiforgery", Tags = ["DisableAntiforgery"])]
[DisableAntiforgery]
public partial class EndpointWithDisableAntiforgery {
    public Task<IResult> HandleAsync() {
        return Task.FromResult<IResult>(TypedResults.Ok(new { Message = "Disable Antiforgery Sample" }));
    }
}

[Endpoint<Get>("/AllowCookieRedirect", Name = "EndpointWithAllowCookieRedirect", Tags = ["AllowCookieRedirect"])]

public partial class EndpointWithAllowCookieRedirect{
    public Task<IResult> HandleAsync() {
        return Task.FromResult<IResult>(TypedResults.Ok(new { Message = "Allow Cookie Redirect Sample" }));
    }
}