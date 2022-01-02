using System;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using MediatR;
using Persistence;

namespace Application.Activities
{
  public class Details
  {
    public class Query : IRequest<Activity>
    {
      //This is what we want to receive as parameter from our API 
      public Guid Id { get; set; }
    }

    public class Handler : IRequestHandler<Query, Activity>
    {
      private readonly DataContext _context;

      public Handler(DataContext context)
      {
        this._context = context;
      }

    // first paramtere is request that is of Query class type and we have there Id property that we want to use
      public async Task<Activity> Handle(Query request, CancellationToken cancellationToken)
      {
        return await _context.Activities.FindAsync(request.Id);
      }
    }
  }
}