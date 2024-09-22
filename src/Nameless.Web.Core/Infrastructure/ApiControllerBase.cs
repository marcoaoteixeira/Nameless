using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace Nameless.Web.Infrastructure;

[ApiController]
[ApiVersion("1")]
[Route("api/v{version:apiVersion}/[controller]")]
public abstract class ApiControllerBase : ControllerBase { }