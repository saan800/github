using Microsoft.AspNetCore.Mvc;

namespace Basic.Api.Controllers;

[ApiController]
[Consumes("application/json")]
[Produces("application/json")]
public abstract class BaseController : ControllerBase
{
}
