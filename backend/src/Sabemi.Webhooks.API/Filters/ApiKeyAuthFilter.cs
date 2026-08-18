using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Sabemi.Webhooks.API.Filters;

public class ApiKeyAuthFilter : IActionFilter
{
    private const string ApiKeyHeader = "X-Api-Key";
    private readonly IConfiguration _config;

    public ApiKeyAuthFilter(IConfiguration config)
    {
        _config = config;
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.HttpContext.Request.Headers.TryGetValue(ApiKeyHeader, out var apiKey))
        {
            context.Result = new UnauthorizedObjectResult(new { message = "Header X-Api-Key ausente" });
            return;
        }

        var configuredKey = _config["ApiKey"];
        if (string.IsNullOrEmpty(configuredKey) || apiKey != configuredKey)
        {
            context.Result = new UnauthorizedObjectResult(new { message = "API Key inválida" });
        }
    }

    public void OnActionExecuted(ActionExecutedContext context) { }
}
