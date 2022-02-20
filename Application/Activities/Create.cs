using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Domain;
using FluentValidation;
using MediatR;
using Persistence;

namespace Application.Activities
{
  public class Create
  {
    //Query return data, command doesn't return data so that's why IRequest is without return type => Commands do not return anything only Queries
    //Command usually don't return anything, but in our case we want to return Result class for error handling, Unit means that we are not returning anything
    public class Command : IRequest<Result<Unit>>
    {
      // This is what we want to receive as parameter from our API
      public Activity Activity { get; set; }
    }

    //Validating against type Command class => because command class has Acitvity
    public class CommandValidator : AbstractValidator<Command>
    {
      public CommandValidator()
      {
        RuleFor(x => x.Activity).SetValidator(new ActivityValidator());
      }
    }

    //We are not returning anything, the first type parameter is Command but we are missing the return type as in Query where was return Activity => aftre code refactoring and adding error handling we have also second parameter
    public class Handler : IRequestHandler<Command, Result<Unit>>
    {
      private readonly DataContext _context;

      public Handler(DataContext context)
      {
        this._context = context;
      }

      //This is implementation code from IRequestHandler
      public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
      {
        //we are just adding to the database so we don't need to use aync version
        _context.Activities.Add(request.Activity);

        var result = await _context.SaveChangesAsync() > 0;

        //this is return nothing, we are just saying to our api contorller that we have finished
        // Task<Unit> is code generated from our IRequestHandler<Command> interface
        if (!result) return Result<Unit>.Failure("Failed to create activity");
        return Result<Unit>.Success(Unit.Value);
      }
    }

  }


}