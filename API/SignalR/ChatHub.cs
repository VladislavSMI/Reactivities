using System;
using System.Threading.Tasks;
using Application.Comments;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{
  public class ChatHub : Hub
  {
    private readonly IMediator _mediator;
    public ChatHub(IMediator mediator)
    {
      this._mediator = mediator;
    }

    public async Task SendComment(Create.Command command)
    {
      //anytime comment is send to that group with acitivityId name, they are going to receive that comment based on the method "ReceiveComment"  
      var comment = await _mediator.Send(command);

      //once comment is saved in our DB and attached all id etc., then we are going to return comment so it can be displayed in uer interface
      await Clients.Group(command.ActivityId.ToString()).SendAsync("ReceiveComment", comment.Value);
    }

    public override async Task OnConnectedAsync()
    {
      //When ever client connects, we are going to join them to the group with the name of activityId and we are going to send them list of comments that we get from our database.
      var httpContext = Context.GetHttpContext();
      var activityId = httpContext.Request.Query["activityId"];
      await Groups.AddToGroupAsync(Context.ConnectionId, activityId);
      var result = await _mediator.Send(new List.Query { ActivityId = Guid.Parse(activityId) });
      await Clients.Caller.SendAsync("LoadComments", result.Value);
    }
  }
}