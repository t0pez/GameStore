using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Web;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace GameStore.Web;

public class Program
{
    public static void Main(string[] args)
    {
        var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

        try
        {
            logger.Debug("init main");
            CreateHostBuilder(args).Build().Run();
        }
        catch (Exception exception)
        {
            logger.Error(exception, "Stopped program because of exception");
            throw;
        }
        finally
        {
            LogManager.Shutdown();
        }
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
                   .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); }).ConfigureLogging(
                       log =>
                       {
                           log.ClearProviders();
                           log.SetMinimumLevel(LogLevel.Trace);
                       }).UseNLog();
    }
}