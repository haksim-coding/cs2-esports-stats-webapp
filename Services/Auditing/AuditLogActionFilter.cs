using System.Globalization;
using System.Reflection;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace cs2_esports.Services.Auditing;

public sealed class AuditLogActionFilter : IAsyncResourceFilter, IAsyncActionFilter, IOrderedFilter
{
    private const string EntityIdItemKey = "cs2scope.audit.entity-id";
    private static readonly HashSet<string> AuditedMethods = new(StringComparer.OrdinalIgnoreCase)
    {
        HttpMethods.Post,
        HttpMethods.Put,
        HttpMethods.Patch,
        HttpMethods.Delete
    };

    private readonly IAuditLogService _auditLogService;

    public AuditLogActionFilter(IAuditLogService auditLogService)
    {
        _auditLogService = auditLogService;
    }

    // Run before ApiController's automatic model validation so rejected writes are audited too.
    public int Order => int.MinValue;

    public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
    {
        if (!AuditedMethods.Contains(context.HttpContext.Request.Method))
        {
            await next();
            return;
        }

        var executedContext = await next();
        var statusCode = GetStatusCode(executedContext);
        var controllerDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
        var entity = controllerDescriptor?.ControllerName ?? context.RouteData.Values["controller"]?.ToString() ?? "Unknown";
        var action = controllerDescriptor?.ActionName ?? context.RouteData.Values["action"]?.ToString() ?? "Unknown";
        var principal = context.HttpContext.User;
        context.HttpContext.Items.TryGetValue(EntityIdItemKey, out var capturedEntityId);

        var entry = new AuditLogEntry
        {
            TimestampUtc = DateTimeOffset.UtcNow,
            Outcome = GetOutcome(executedContext, statusCode),
            HttpMethod = context.HttpContext.Request.Method,
            Entity = entity,
            Action = action,
            EntityId = capturedEntityId?.ToString() ??
                FindEntityId(context, executedContext),
            ActorName = principal.Identity?.IsAuthenticated == true ? principal.Identity.Name : null,
            ActorId = principal.FindFirstValue(ClaimTypes.NameIdentifier),
            ActorRoles = principal.FindAll(ClaimTypes.Role)
                .Select(claim => claim.Value)
                .Distinct(StringComparer.Ordinal)
                .OrderBy(role => role, StringComparer.Ordinal)
                .ToArray(),
            Path = context.HttpContext.Request.Path.Value ?? string.Empty,
            StatusCode = statusCode,
            TraceId = context.HttpContext.TraceIdentifier,
            RemoteIpAddress = context.HttpContext.Connection.RemoteIpAddress?.ToString(),
            ErrorType = executedContext.Exception?.GetType().FullName
        };

        await _auditLogService.WriteAsync(entry, CancellationToken.None);
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var executedContext = await next();
        var entityId = FindEntityId(context, executedContext);
        if (!string.IsNullOrWhiteSpace(entityId))
        {
            context.HttpContext.Items[EntityIdItemKey] = entityId;
        }
    }

    private static int GetStatusCode(ResourceExecutedContext context)
    {
        if (context.Exception is not null && !context.ExceptionHandled)
        {
            return StatusCodes.Status500InternalServerError;
        }

        return context.Result switch
        {
            IStatusCodeActionResult statusCodeResult when statusCodeResult.StatusCode.HasValue => statusCodeResult.StatusCode.Value,
            _ when context.HttpContext.Response.StatusCode > 0 => context.HttpContext.Response.StatusCode,
            _ => StatusCodes.Status200OK
        };
    }

    private static string GetOutcome(ResourceExecutedContext context, int statusCode)
    {
        if ((context.Exception is not null && !context.ExceptionHandled) || statusCode >= 500)
        {
            return "Failed";
        }

        return statusCode >= 400 || !context.ModelState.IsValid ? "Rejected" : "Succeeded";
    }

    private static string? FindEntityId(ActionExecutingContext executing, ActionExecutedContext executed)
    {
        executing.ActionArguments.TryGetValue("id", out var actionId);
        if (TryFormatIdentifier(executing.RouteData.Values["id"], out var identifier) ||
            TryFormatIdentifier(actionId, out identifier))
        {
            return identifier;
        }

        foreach (var argument in executing.ActionArguments.Values.Where(value => value is not null))
        {
            var idProperty = argument!.GetType().GetProperty("Id", BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
            if (idProperty is not null && TryFormatIdentifier(idProperty.GetValue(argument), out identifier))
            {
                return identifier;
            }
        }

        if (executed.Result is RedirectToActionResult redirect &&
            TryFormatIdentifier(redirect.RouteValues?["id"], out identifier))
        {
            return identifier;
        }

        if (executed.Result is CreatedAtActionResult created &&
            TryFormatIdentifier(created.RouteValues?["id"], out identifier))
        {
            return identifier;
        }

        if (executed.Result is ObjectResult objectResult && objectResult.Value is not null)
        {
            var idProperty = objectResult.Value.GetType().GetProperty("Id", BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
            if (idProperty is not null && TryFormatIdentifier(idProperty.GetValue(objectResult.Value), out identifier))
            {
                return identifier;
            }
        }

        return TryFormatIdentifier(executing.RouteData.Values["slug"], out identifier) ? identifier : null;
    }

    private static string? FindEntityId(ResourceExecutingContext executing, ResourceExecutedContext executed)
    {
        if (TryFormatIdentifier(executing.RouteData.Values["id"], out var identifier))
        {
            return identifier;
        }

        if (executed.Result is RedirectToActionResult redirect &&
            TryFormatIdentifier(redirect.RouteValues?["id"], out identifier))
        {
            return identifier;
        }

        if (executed.Result is CreatedAtActionResult created &&
            TryFormatIdentifier(created.RouteValues?["id"], out identifier))
        {
            return identifier;
        }

        return TryFormatIdentifier(executing.RouteData.Values["slug"], out identifier) ? identifier : null;
    }

    private static bool TryFormatIdentifier(object? value, out string? identifier)
    {
        identifier = value switch
        {
            null => null,
            string text when !string.IsNullOrWhiteSpace(text) => text,
            IFormattable formattable => formattable.ToString(null, CultureInfo.InvariantCulture),
            _ => value.ToString()
        };
        return !string.IsNullOrWhiteSpace(identifier);
    }
}
