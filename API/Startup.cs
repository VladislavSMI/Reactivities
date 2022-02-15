using API.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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

      services.AddControllers();
      services.AddApplicationServices(_config);

    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1"));
      }

      //if we are not ussing https in development then we can comment this out
      // app.UseHttpsRedirection();

      app.UseRouting();

      app.UseCors("CorsPolicy");

      app.UseAuthorization();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();
      });
    }
  }
}
