using Application.Core;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace API.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class BaseApiController : ControllerBase
  {
    // We are bringing mediator to your base controller => then all controller will have access to this

    //this is private field for IMediator
    private IMediator _mediator;
    //protected means that it will be available to any derived class and BaseApiController itself
    //method being created in new way without need of return statement
    // return from this method _mediator (if we have it available) or ??= it means that if _mediator is null then set it up to HttpContext.RequestServices.GetService<IMediator>()
    //HttpContext.RequestServices.GetService<IMediator>() this means that we are requesting mediator services that were added in ApplicationServiceExtensions 
    protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>();


    protected ActionResult HandleResult<T>(Result<T> result)
    {
      if (result == null)
      {
        return NotFound();
      }
      if (result.IsSuccess && result.Value != null)
      {
        return Ok(result.Value);
      }
      if (result.IsSuccess && result.Value == null)
      {
        return NotFound();
      }
      return BadRequest(result.Error);
    }
  }
}