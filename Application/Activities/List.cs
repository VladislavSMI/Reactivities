using System.Collections.Generic;
using System.Threading;
using MediatR;
using System.Threading.Tasks;
using Persistence;
using Microsoft.EntityFrameworkCore;
using Application.Core;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Application.Interfaces;

namespace Application.Activities
{
  public class List
  {
    //We are creating Query class inside of List class that is based on mediator IRequest interface with type List
    // Query are returning values <List<Activity>> vs Command are not returning anything
    public class Query : IRequest<Result<List<ActivityDto>>>
    {

      // If we had any parameters, that we have to pass to this Query, then they can go inside here as class properties.
      //In this case we are simply returning list so nothing goes here 
    }

    // Handler class => in our handler we want to get access to our data context => we will construct that
    // It has 2 type parameters => first one is query and second one is what we are going to return 
    public class Handler : IRequestHandler<Query, Result<List<ActivityDto>>>
    {
      //this is field that will be initialized in constructor
      private readonly DataContext _context;
      private readonly IMapper _mapper;
      private readonly IUserAccessor _userAccessor;

      public Handler(DataContext context, IMapper mapper, IUserAccessor userAccessor)
      {
        this._userAccessor = userAccessor;
        this._mapper = mapper;
        this._context = context;
      }

      //This was implemented by IRequestHandler interface
      public async Task<Result<List<ActivityDto>>> Handle(Query request, CancellationToken cancellationToken)
      {

        //Before implementing mediator pattern => this was in activities controller in our API => but based on the clean architecture principles => we have moved this logic to our Application layer.
        //Before we execute to list, we will specify include statement, where will specify that we want to get our attendees, atendess are just our join table, and our attendees are related to our app user. So we also want to get our app users.
        // This was less efficient code as we were quering more data then nedded 
        // var activities = await _context.Activities.Include(a => a.Attendees).ThenInclude(u => u.AppUser).ToListAsync(cancellationToken);
        // Instead of include we will use automapper method ProjectTo from AutoMapper. We could have it done as well with Select() from Ling, but AutoMapper is easier
        var activities = await _context.Activities.ProjectTo<ActivityDto>(_mapper.ConfigurationProvider, new { currentUsername = _userAccessor.GetUserName() }).ToListAsync(cancellationToken);

        //with new ProjectTo we are already returning ActivityDto we don't need to map from activities to ActivityDto
        // var activitiesToReturn = _mapper.Map<List<ActivityDto>>(activities);
        return Result<List<ActivityDto>>.Success(activities);
      }
    }
  }
}