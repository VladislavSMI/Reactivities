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
    //We are creating Query class inside of List class that is based on mediator IRequest interface with type List
    // Query are returning values <List<Activity>> vs Command are not returning anything
    public class Query : IRequest<List<Activity>>
    {

      // If we had any parameters, that we have to pass to this Query, then they can go inside here as class properties.
      //In this case we are simply returning list so nothing goes here 
    }

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

      //This was implemented by IRequestHandler interface
      public async Task<List<Activity>> Handle(Query request, CancellationToken cancellationToken)
      {

        //Before implementing mediator pattern => this was in activities controller in our API => but based on the clean architecture principles => we have moved this logic to our Application layer.
        return await _context.Activities.ToListAsync();
      }
    }
  }
}