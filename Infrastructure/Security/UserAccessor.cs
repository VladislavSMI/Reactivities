using System.Security.Claims;
using Application.Interfaces;
using Microsoft.AspNetCore.Http;


//We are accessing User object via httpContextAccessor => we have to add this as a service to ApplicationServiceExtensions.cs
namespace Infrastructure.Security
{
  public class UserAccessor : IUserAccessor
  {
    private readonly IHttpContextAccessor _httpContextAccessor;
    //WE have access to http context via this accessor, it is constructor dependency injection
    public UserAccessor(IHttpContextAccessor httpContextAccessor)
    {
      this._httpContextAccessor = httpContextAccessor;
    }

    public string GetUserName()
    {
      return _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
    }
  }
}