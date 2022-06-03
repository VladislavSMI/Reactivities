using System.Linq;
using Application.Activities;
using Application.Comments;
using AutoMapper;
using Domain;

namespace Application.Core
{
  public class MappingProfiles : Profile
  {
    public MappingProfiles()
    {
      string currentUsername = null;
      //We are maping from Activity to Activity and then we have to add it as service to our Startup class
      CreateMap<Activity, Activity>();

      CreateMap<Activity, ActivityDto>()
      .ForMember(d => d.HostUsername, o => o.MapFrom(s => s.Attendees.FirstOrDefault(x => x.IsHost).AppUser.UserName));
      // Here we have to be specific with Profiles.Profile class, becuase automapper has also Profile class, which is our MappingProfiles class based on
      //d is destination member
      // s is source we are mapping from 
      CreateMap<ActivityAttendee, AttendeeDto>()
      .ForMember(d => d.DisplayName, o => o.MapFrom(s => s.AppUser.DisplayName))
      .ForMember(d => d.UserName, o => o.MapFrom(s => s.AppUser.UserName))
      .ForMember(d => d.Bio, o => o.MapFrom(s => s.AppUser.Bio)).ForMember(d => d.Image, o => o.MapFrom(s => s.AppUser.Photos.FirstOrDefault(x => x.IsMain).Url))
      .ForMember(d => d.FollowersCount, o => o.MapFrom(s => s.AppUser.Followers.Count))
      .ForMember(d => d.FollowingCount, o => o.MapFrom(s => s.AppUser.Followings.Count))
      .ForMember(d => d.Following, o => o.MapFrom(s => s.AppUser.Followers.Any(x => x.Observer.UserName == currentUsername)));

      CreateMap<AppUser, Profiles.Profile>()
      .ForMember(d => d.Image, o => o.MapFrom(s => s.Photos.FirstOrDefault(x => x.IsMain).Url))
      .ForMember(d => d.FollowersCount, o => o.MapFrom(s => s.Followers.Count))
      .ForMember(d => d.FollowingCount, o => o.MapFrom(s => s.Followings.Count))
      //Here we want to know if currently loogged in user is inside Followers Collection, we need to get access ot our currently logged in user information from our token, but we can't inject anything to this class, but from our  Application.Followers List class we can pass parmas to our configuration and that's how we will get it to Mapping profiles. Inside mapping profiles constructor we will add string currentUsername = null 
      .ForMember(d => d.Following, o => o.MapFrom(s => s.Followers.Any(x => x.Observer.UserName == currentUsername)));


      CreateMap<Comment, CommentDto>()
      .ForMember(d => d.DisplayName, o => o.MapFrom(s => s.Author.DisplayName))
      .ForMember(d => d.UserName, o => o.MapFrom(s => s.Author.UserName))
      .ForMember(d => d.Image, o => o.MapFrom(s => s.Author.Photos.FirstOrDefault(x => x.IsMain).Url));
    }
  }
}