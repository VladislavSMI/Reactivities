using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Activities;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace API.Controllers
{
  public class ActivitiesController : BaseApiController
  {

    [HttpGet]
    public async Task<ActionResult<List<Activity>>> GetActivities()
    {
      return await Mediator.Send(new List.Query());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Activity>> GetActivity(Guid id)
    {
      //{Id = id} object initilizer that sets Id property in Details.Query Class to id from HttpGet request => it is short hand syntax for calling a constructor
      return await Mediator.Send(new Details.Query { Id = id });
    }

    [HttpPost]
    // Return type IActionResult will give us access to return such as 200 OK, 401 etc. => but we are not actually return any data from database
    public async Task<IActionResult> CreateActivity(Activity activity)
    {
      return Ok(await Mediator.Send(new Create.Command { Activity = activity }));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> EditActivity(Guid id, Activity activity)
    {
      activity.Id = id;
      return Ok(await Mediator.Send(new Edit.Command { Activity = activity }));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteActivity(Guid id)
    {
      return Ok(await Mediator.Send(new Delete.Command { Id = id }));
    }

  }
}