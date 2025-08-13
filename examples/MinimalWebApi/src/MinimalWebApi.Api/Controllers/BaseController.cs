using Microsoft.AspNetCore.Mvc;

namespace MinimalWebApi.Api.Controllers;

[ApiController]
[Consumes("application/json")]
[Produces("application/json")]
public abstract class BaseController : ControllerBase
{
}
