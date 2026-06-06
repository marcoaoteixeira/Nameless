using Nameless.Web.Http.Endpoints;

namespace Nameless.Microservices.Api.Domains.Chores.UseCases;

[EndpointGroup(prefix: "/api/v{version:apiVersion}/chores", Tags = ["Chores"])]
public partial class ChoresEndpointGroup;
