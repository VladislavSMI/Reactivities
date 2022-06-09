using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Domain
{
  public class AppUser : IdentityUser

  //as we are deriving from IdentityUser class, we got access to all aditional properties such as UserName, email adress etc. 
  {
    public string DisplayName { get; set; }
    public string Bio { get; set; }
    //connection to join table ActiviyAttnedee.cs
    public ICollection<ActivityAttendee> Activities { get; set; }
    // connection to join table Photo.cs
    public ICollection<Photo> Photos { get; set; }
    public ICollection<UserFollowing> Followings { get; set; }
    public ICollection<UserFollowing> Followers { get; set; }

    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
  }
}