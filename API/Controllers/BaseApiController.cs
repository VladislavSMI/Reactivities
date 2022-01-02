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
    // ??= it means that _mediator is null then set it up to right side
    protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>();

  }
}