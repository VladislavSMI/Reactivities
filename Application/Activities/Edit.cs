using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Domain;
using MediatR;
using Persistence;

namespace Application.Activities
{
  public class Edit
  {
    public class Command : IRequest
    {
      public Activity Activity { get; set; }
    }

    public class Handler : IRequestHandler<Command>
    {
      private readonly DataContext _context;
      private readonly IMapper _mapper;

      public Handler(DataContext context, IMapper mapper)
      {
        this._context = context;
        this._mapper = mapper;
      }

      public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
      {
        var activity = await _context.Activities.FindAsync(request.Activity.Id);
        
        //request.Activity.Id => we are adding Id in Controllers 
        //thanks to this nuget package we are maping properties of request.Activity into activity object that we got from database
        _mapper.Map(request.Activity, activity);

        await _context.SaveChangesAsync();

        return Unit.Value;
      }
    }


  }
}