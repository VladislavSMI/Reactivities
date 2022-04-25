using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Interfaces;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Photos
{
  public class Add
  {
    public class Command : IRequest<Result<Photo>>
    {
      public IFormFile File { get; set; }
    }

    public class Handler : IRequestHandler<Command, Result<Photo>>
    {
      private readonly DataContext _context;
      private readonly IPhotoAccessor _photoAccessor;
      private readonly IUserAccessor _userAccessor;
      public Handler(DataContext context, IPhotoAccessor photoAccessor, IUserAccessor userAccessor)
      {
        this._userAccessor = userAccessor;
        this._photoAccessor = photoAccessor;
        this._context = context;
      }

      public async Task<Result<Photo>> Handle(Command request, CancellationToken cancellationToken)
      {
        //   First thing we are going to get our user from db and only currently logged in user can add photo to that user's photo collection
        var user = await _context.Users.Include(p => p.Photos).FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUserName());

        if (user == null) return null;

        // Then we are going to try to add our photo to cloudinary
        // It has to be called File

        var PhotoUploadResult = await _photoAccessor.AddPhoto(request.File);

        //If these fails, then our AddPhoto method will throw exception so we don't have to check here
        //here we are accessing PhotoAccessor.cs file in Infrastructure via our IPhotoAccessor interface in Application layer
        var photoUploadResult = await _photoAccessor.AddPhoto(request.File);

        var photo = new Photo
        {
          Url = photoUploadResult.Url,
          Id = photoUploadResult.PublicId
        };

        // We will check if this is the first photo user is uploading
        if (!user.Photos.Any(x => x.IsMain)) photo.IsMain = true;

        // Then we will add photo to user collection
        user.Photos.Add(photo);

        var result = await _context.SaveChangesAsync() > 0;

        if (result) return Result<Photo>.Success(photo);

        return Result<Photo>.Failure("Problem adding photo");
      }
    }
  }
}