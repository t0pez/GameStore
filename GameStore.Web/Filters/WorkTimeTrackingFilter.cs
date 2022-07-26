using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace GameStore.Web.Filters;

public class WorkTimeTrackingFilter : IActionFilter
{
    private readonly ILogger<WorkTimeTrackingFilter> _logger;
    private readonly Stopwatch _stopwatch = new();

    public WorkTimeTrackingFilter(ILogger<WorkTimeTrackingFilter> logger)
    {
        _logger = logger;
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        _stopwatch.Start();
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        _stopwatch.Stop();
        _logger.LogInformation("Executed in {milliseconds}ms", _stopwatch.ElapsedMilliseconds);
    }
}