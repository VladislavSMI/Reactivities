using API.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using FluentValidation.AspNetCore;
using Application.Activities;
using API.Middleware;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using API.SignalR;

namespace API
{
  public class Startup
  {

    // Field and it will be initialized in the constructor
    //Configuration is what is specified in configuration files => appsettings.Development.json and appsettings.json
    //It is just convention to use underscore for private fields 
    private readonly IConfiguration _config;
    public Startup(IConfiguration config)
    {
      _config = config;
    }


    // This method gets called by the runtime. Use this method to add services to the container. It is refered to as dependecy injection container
    public void ConfigureServices(IServiceCollection services)
    {

      services.AddControllers(opt =>
      {
        var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
        opt.Filters.Add(new AuthorizeFilter(policy));
      }
      ).AddFluentValidation(config =>
      {
        config.RegisterValidatorsFromAssemblyContaining<Create>();
      });
      services.AddApplicationServices(_config);
      services.AddIdentityServices(_config);

    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      //We have created this custom middleware to help us handle exception eg. in Delete.cs when activity to delete doesn't exist
      app.UseMiddleware<ExceptionMiddleware>();

      //Security settings via package NWebsec.AspNetCore.Middleware
      app.UseXContentTypeOptions();
      app.UseReferrerPolicy(opt => opt.NoReferrer());
      app.UseXXssProtection(opt => opt.EnabledWithBlockMode());
      app.UseXfo(opt => opt.Deny());
      app.UseCsp(opt => opt
            .BlockAllMixedContent()
            .StyleSources(s => s.Self().CustomSources("https://fonts.googleapis.com", "https://cdn.jsdelivr.net"))
            .FontSources(s => s.Self().CustomSources("https://fonts.gstatic.com", "https://cdn.jsdelivr.net", "data:"))
            .FormActions(s => s.Self())
            .FrameAncestors(s => s.Self())
            .ImageSources(s => s.Self().CustomSources("https://res.cloudinary.com"))
            .ScriptSources(s => s.Self())
      );

      if (env.IsDevelopment())
      {
        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1"));
      }

      else
      {
        app.Use(async (context, next) =>
        {
          context.Response.Headers.Add("Strict-Transport-Security", "max-age=31536000");
          await next.Invoke();
        });
      }

      //if we are not ussing https in development then we can comment this out
      // app.UseHttpsRedirection();

      app.UseRouting();
      //this 2 are for React app
      app.UseDefaultFiles();
      app.UseStaticFiles();

      app.UseCors("CorsPolicy");

      //UserAuthentication has to go before UseAuthorization
      app.UseAuthentication();
      app.UseAuthorization();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();
        endpoints.MapHub<ChatHub>("/chat");
        //this endpoint is for react routes
        endpoints.MapFallbackToController("Index", "Fallback");
      });
    }
  }
}
