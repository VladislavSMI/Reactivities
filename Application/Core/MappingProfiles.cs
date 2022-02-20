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
    }
  }
}