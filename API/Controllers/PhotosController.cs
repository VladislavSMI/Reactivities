using System.Threading.Tasks;
using Application.Photos;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
  public class PhotosController : BaseApiController
  {
    [HttpPost]
    //[FromForm] => this is atribute, we will use this atribute to tell our api controller where to find the file, the command will include File
    public async Task<IActionResult> Add([FromForm] Add.Command command)
    {
      return HandleResult(await Mediator.Send(command));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
      return HandleResult(await Mediator.Send(new Delete.Command { Id = id }));
    }

    [HttpPost("{id}/setMain")]
    public async Task<IActionResult> SetMain(string id)
    {
      return HandleResult(await Mediator.Send(new SetMain.Command { Id = id }));
    }
  }
}