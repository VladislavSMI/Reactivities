using System.Collections.Generic;
using System.Threading;
using Domain;
using MediatR;
using System.Threading.Tasks;
using Persistence;
using Microsoft.EntityFrameworkCore;

namespace Application.Activities
{
  public class List
  {
    //We are creating Query class inside of List class that is based on mediator IRequest with type List
    // Query are returning values <List<Activity>> vs Command are not returning anything
    public class Query : IRequest<List<Activity>> { }

    // Handler class => in our handler we want to get access to our data context => we will construct that
    // It has 2 type parameters => first one is query and second one is what we are going to return 
    public class Handler : IRequestHandler<Query, List<Activity>>
    {
      //this is field that will be initialized in constructor
      private readonly DataContext _context;

      public Handler(DataContext context)
      {
        this._context = context;
      }

      public async Task<List<Activity>> Handle(Query request, CancellationToken cancellationToken)
      {
        

        return await _context.Activities.ToListAsync(cancellationToken);
      }
    }
  }
}