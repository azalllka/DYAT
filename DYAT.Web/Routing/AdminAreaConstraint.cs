using Microsoft.AspNetCore.Routing;

namespace DYAT.Web.Routing;

public class AdminAreaConstraint : IRouteConstraint
{
    public bool Match(HttpContext? httpContext, IRouter? route, string routeKey, 
        RouteValueDictionary values, RouteDirection routeDirection)
    {
        if (values.TryGetValue("area", out var area))
        {
            if (area?.ToString()?.Equals("Admin", StringComparison.OrdinalIgnoreCase) == true)
            {
                return httpContext?.User?.IsInRole("Admin") ?? false;
            }
        }
        return true;
    }
} 