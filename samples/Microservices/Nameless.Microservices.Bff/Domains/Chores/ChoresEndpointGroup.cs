using Microsoft.AspNetCore.Authorization;
using Nameless.Web.Http.Endpoints;

namespace Nameless.Microservices.Bff.Domains.Chores;

[EndpointGroup(prefix: "/bff/chores", Tags = ["Chores"])]
[Authorize]
public partial class ChoresEndpointGroup;
