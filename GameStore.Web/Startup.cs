using GameStore.Infrastructure;
using GameStore.Infrastructure.Data.Context;
using GameStore.Web.Converters;
using GameStore.Web.Middlewares;
using GameStore.Web.Profiles;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text.Json.Serialization;
using AutoMapper;
using GameStore.Core.Profiles;
using GameStore.Web.Filters;
using GameStore.Web.Infrastructure;
using Quartz;

namespace GameStore.Web;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddMvc();
        
        services.AddControllers()
                .AddJsonOptions(options =>
                                {
                                    options.JsonSerializerOptions.Converters.Add(
                                        new ByteArrayJsonConverter());
                                    options.JsonSerializerOptions.ReferenceHandler =
                                        ReferenceHandler.IgnoreCycles;
                                });

        services.AddQuartz(configurator =>
                           {
                               configurator.UseMicrosoftDependencyInjectionJobFactory();
                               configurator.AddOrderTimeOutHostedService();
                           });
        services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

        services.ConfigureDomainServices();
        services.ConfigureWebServices();

        services.AddScoped<WorkTimeTrackingFilter>();

        services.AddDbContext<ApplicationContext>(options =>
                                                  {
                                                      var connectionString =
                                                          Configuration.GetConnectionString("DefaultConnection");
                                                      options.UseSqlServer(connectionString);
                                                  });

        services.AddAutoMapper(
            configuration => configuration.AddProfiles(new Profile[] { new WebCommonProfile(), new CoreCommonProfile() }));
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ApplicationContext dbContext)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseStaticFiles();
        app.UseCookiePolicy();

        app.UseMiddleware<ExceptionMiddleware>();

        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
}