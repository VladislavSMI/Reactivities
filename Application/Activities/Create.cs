using System.Threading;
using System.Threading.Tasks;
using Domain;
using MediatR;
using Persistence;

namespace Application.Activities
{
  public class Create
  {
    //Query return data, command doesn't return data so that's why IRequest is without return type => Commands do not return anything only Queries
    public class Command : IRequest
    {
      // This is what we want to receive as parameter from our API
      public Activity Activity { get; set; }
    }

    //We are not returning anything, the first type parameter is Command but we are missing the return type as in Query where was return Activity
    public class Handler : IRequestHandler<Command>
    {
      private readonly DataContext _context;

      public Handler(DataContext context)
      {
        this._context = context;
      }

      //This is implementation code from IRequestHandler
      public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
      {
        //we are just adding to the database so we don't need to use aync version
        _context.Activities.Add(request.Activity);

        await _context.SaveChangesAsync();

        //this is return nothing, we are just saying to our api contorller that we have finished
        // Task<Unit> is code generated from our IRequestHandler<Command> interface
        return Unit.Value;
      }
    }

  }


}