using System;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using MediatR;
using Persistence;
using Application.Core;

namespace Application.Activities
{
  public class Details
  {
    public class Query : IRequest<Result<Activity>>
    {
      //This is what we want to receive as parameter from our API 
      public Guid Id { get; set; }
    }

    //Handler has 2 parameters => first one input and second one is Return 
    public class Handler : IRequestHandler<Query, Result<Activity>>
    {
      private readonly DataContext _context;

      public Handler(DataContext context)
      {
        this._context = context;
      }

      // first paramtere is request that is of Query class type and we have there Id property that we want to use
      public async Task<Result<Activity>> Handle(Query request, CancellationToken cancellationToken)
      {
        var activity = await _context.Activities.FindAsync(request.Id);

        return Result<Activity>.Success(activity);
      }
    }
  }
}