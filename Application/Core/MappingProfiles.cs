using System.Linq;
using Application.Activities;
using AutoMapper;
using Domain;

namespace Application.Core
{
  public class MappingProfiles : Profile
  {
    public MappingProfiles()
    {
      //We are maping from Activity to Activity and then we have to add it as service to our Startup class
      CreateMap<Activity, Activity>();
      CreateMap<Activity, ActivityDto>()
      .ForMember(d => d.HostUsername, o => o.MapFrom(s => s.Attendees.FirstOrDefault(x => x.IsHost).AppUser.UserName));
      // Here we have to be specific with Profiles.Profile class, becuase automapper has also Profile class, which is our MappingProfiles class based on
      CreateMap<ActivityAttendee, AttendeeDto>()
      .ForMember(d => d.DisplayName, o => o.MapFrom(s => s.AppUser.DisplayName))
      .ForMember(d => d.UserName, o => o.MapFrom(s => s.AppUser.UserName))
      .ForMember(d => d.Bio, o => o.MapFrom(s => s.AppUser.Bio)).ForMember(d => d.Image, o => o.MapFrom(s => s.AppUser.Photos.FirstOrDefault(x => x.IsMain).Url));
      CreateMap<AppUser, Profiles.Profile>()
      .ForMember(d => d.Image, o => o.MapFrom(s => s.Photos.FirstOrDefault(x => x.IsMain).Url));
    }
  }
}