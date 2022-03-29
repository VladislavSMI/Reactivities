using System;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using MediatR;
using Persistence;
using Application.Core;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace Application.Activities
{
  public class Details
  {
    public class Query : IRequest<Result<ActivityDto>>
    {
      //This is what we want to receive as parameter from our API 
      public Guid Id { get; set; }
    }

    //Handler has 2 parameters => first one input and second one is Return 
    public class Handler : IRequestHandler<Query, Result<ActivityDto>>
    {
      private readonly DataContext _context;
      private readonly IMapper _mapper;

      public Handler(DataContext context, IMapper mapper)
      {
        this._mapper = mapper;
        this._context = context;
      }

      // first paramtere is request that is of Query class type and we have there Id property that we want to use
      public async Task<Result<ActivityDto>> Handle(Query request, CancellationToken cancellationToken)
      {
        var activity = await _context.Activities
        .ProjectTo<ActivityDto>(_mapper.ConfigurationProvider).FirstOrDefaultAsync(x => x.Id == request.Id);

        return Result<ActivityDto>.Success(activity);
      }
    }
  }
}