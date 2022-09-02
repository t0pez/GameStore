using GameStore.Core.Services.HostedServices;
using Quartz;

namespace GameStore.Web.Infrastructure;

public static class QuartsConfiguratorExtensions
{
    public static void AddOrderTimeOutHostedService(this IServiceCollectionQuartzConfigurator configurator)
    {
        const string jobName = nameof(OrderTimeOutJob) + "JobName";
        const string jobTriggerName = nameof(OrderTimeOutJob) + "TriggerName";

        var jobKey = new JobKey(jobName);
        configurator.AddJob<OrderTimeOutJob>(options => options.WithIdentity(jobKey));

        configurator.AddTrigger(options =>
                                    options.WithIdentity(jobTriggerName)
                                           .ForJob(jobKey)
                                           .StartNow()
                                           .WithCronSchedule("* * * * * ?"));
    }
}