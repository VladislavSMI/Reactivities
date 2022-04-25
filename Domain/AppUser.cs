using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Domain
{
  public class AppUser : IdentityUser
  {
    public string DisplayName { get; set; }
    public string Bio { get; set; }
    //connection to join table ActiviyAttnedee.cs
    public ICollection<ActivityAttendee> Activities { get; set; }
    // connection to join table Photo.cs
    public ICollection<Photo> Photos { get; set; }
  }
}