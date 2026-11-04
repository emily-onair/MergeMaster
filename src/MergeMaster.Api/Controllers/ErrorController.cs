using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace MergeMaster.Api.Controllers;

[ApiController]
[Route("/Error")]
public class ErrorController : Controller
{
    public IActionResult Error()
    {
        var feature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
        // Log feature.Error, return a view or ProblemDetails, etc.
        return Problem(title: "An error occurred.");
    }
}   