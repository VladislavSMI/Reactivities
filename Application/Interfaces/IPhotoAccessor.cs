using System.Threading.Tasks;
using Application.Photos;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces
{
    public interface IPhotoAccessor
    {
        // These two methods are not going to touch our Db, they are purely for adding and deleting files from coulidary
         Task<PhotoUploadResult> AddPhoto(IFormFile file);
         Task<string> DeletePhoto(string publicId);

    }
}