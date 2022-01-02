using Application.Activities;
using Application.Activities.Core;
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
    //We are extending IServiceCollection with additional methods => that why we have to use in parameter this.IServiceCollection services
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
    {
      services.AddSwaggerGen(c =>
          {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
          });

      services.AddDbContext<DataContext>(opt =>
      {
        opt.UseSqlite(config.GetConnectionString("DefaultConnection"));
      });

      //we have to add it to our middleware in Configure method
      services.AddCors(opt =>
      {
        opt.AddPolicy("CorsPolicy", policy =>
        {
          //once we deploy ourapplication, this will become irrelevant as we will be serving our appliction from same domain
          policy.AllowAnyMethod().AllowAnyHeader().WithOrigins("http://localhost:3000");
        });
      });

      //this will tell our mediator where to find our handlers => Matiator is nuget package
      services.AddMediatR(typeof(List.Handler).Assembly);
      // AutoMapper is nuget package that will help us with edit putHttprequest to update activity => it will help us to map object properties from one object into another
      services.AddAutoMapper(typeof(MappingProfiles).Assembly);

      return services;
    }
  }
}