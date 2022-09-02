using System.Net;
using System.Threading.Tasks;
using GameStore.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.Web.Controllers;

public class ErrorController : Controller
{
    private const string AccessDeniedMessage = "Not enough permissions for this page";

    [Route("/error")]
    public async Task<ActionResult> ErrorAsync([FromQuery] int statusCode, [FromQuery] string message)
    {
        ErrorViewModel errorViewModel = new()
        {
            Description = message
        };

        switch (statusCode)
        {
            case (int)HttpStatusCode.NotFound:
                errorViewModel.ErrorName = "Item not found";

                break;
            case (int)HttpStatusCode.Unauthorized:
                return RedirectToAction("AccessDenied", "Error");
            default:
                errorViewModel.ErrorName = "Error";

                break;
        }

        return View(errorViewModel);
    }

    [Route("/forbidden")]
    public async Task<ActionResult> AccessDeniedAsync()
    {
        ErrorViewModel errorViewModel = new()
        {
            ErrorName = "Forbidden",
            Description = AccessDeniedMessage
        };

        return View("Error", errorViewModel);
    }
}