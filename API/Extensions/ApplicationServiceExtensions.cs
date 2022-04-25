using Application.Activities;
using Application.Core;
using Application.Interfaces;
using Infrastructure.Photos;
using Infrastructure.Security;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Persistence;

namespace API.Extensions
{
  //this class will create services that will be inherited and used in our Startup class
  //It is extension class => best way is to declare it as static so we don't have to create new instance of this class when we use our extension methods
  public static class ApplicationServiceExtensions
  {
    //We are extending IServiceCollection with additional methods => that's why we have to use in parameter this.IServiceCollection services
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
    {
      services.AddSwaggerGen(c =>
          {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
          });

      //AdddDbContext => we are getting it from entity framwork, type will be class of our database DataContext.cs
      services.AddDbContext<DataContext>(opt =>
      {
        //config will get info from appsettings.Development.json or appsettings.json => we have specified there that "DefaultConnection": "Data source=reactivities.db" 
        opt.UseSqlite(config.GetConnectionString("DefaultConnection"));
      });

      //we have to add it to our middleware in Configure method
      services.AddCors(opt =>
      {
        opt.AddPolicy("CorsPolicy", policy =>
        {
          //once we deploy ourapplication, this will become irrelevant as we will be serving our appliction from same domain
          // policy.AllowAnyMethod().AllowAnyHeader().WithOrigins("http://localhost:3000");
          policy.AllowAnyMethod().AllowAnyHeader().AllowAnyOrigin();
        });
      });

      //this will tell our mediator where to find our handlers => Matiator is nuget package
      services.AddMediatR(typeof(List.Handler).Assembly);
      // AutoMapper is nuget package that will help us with edit putHttprequest to update activity => it will help us to map object properties from one object into another
      services.AddAutoMapper(typeof(MappingProfiles).Assembly);
      //Here we are adding service that will allow us to access currently logged in user => method is defined in Security UserAccessor.cs, first we have to use interface IUserAccessor and its implementation in UserAccessor. IUserAccessor is defined in Application layer as interface. UserAccessor is defined in Infrastructure. With this service we got the ability to got our currently logged in user name from anywhere in the application as everything is connected to API layer. 
      services.AddScoped<IUserAccessor, UserAccessor>();
      // The same logic as with IUserAccessor and UserAccessor
      // PhotoAccessor is defined in Infrastrucuture project, we are accessing data in our Application layer via interface IPhotoAccessor and then connecting them here in ApplicationServiceExtensions.cs class
      services.AddScoped<IPhotoAccessor, PhotoAccessor>();
      services.Configure<CloudinarySettings>(config.GetSection("Cloudinary"));

      return services;
    }
  }
}