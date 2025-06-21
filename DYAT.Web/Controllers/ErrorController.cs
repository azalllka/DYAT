using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace DYAT.Web.Controllers;

public class ErrorController : Controller
{
    private readonly ILogger<ErrorController> _logger;

    public ErrorController(ILogger<ErrorController> logger)
    {
        _logger = logger;
    }

    [Route("Error")]
    public IActionResult Error(int? statusCode = null)
    {
        var errorFeature = HttpContext.Features.Get<IExceptionHandlerFeature>();
        if (errorFeature != null)
        {
            _logger.LogError($"Error occurred: {errorFeature.Error}");
        }

        if (statusCode.HasValue)
        {
            Response.StatusCode = statusCode.Value;
        }

        return View();
    }
} 