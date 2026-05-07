namespace cs2_esports.Helpers;

public static class RouteSlugHelper
{
    public static string ToRouteSegment(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        var builder = new System.Text.StringBuilder(value.Length);
        var lastWasSeparator = false;

        foreach (var character in value.Trim())
        {
            if (char.IsLetterOrDigit(character))
            {
                builder.Append(character);
                lastWasSeparator = false;
            }
            else if (!lastWasSeparator && builder.Length > 0)
            {
                builder.Append('-');
                lastWasSeparator = true;
            }
        }

        return builder.ToString().Trim('-');
    }

    public static bool MatchesRouteSegment(string? value, string? routeSegment)
    {
        if (string.IsNullOrWhiteSpace(value) || string.IsNullOrWhiteSpace(routeSegment))
        {
            return false;
        }

        return string.Equals(ToRouteSegment(value), routeSegment.Trim(), StringComparison.OrdinalIgnoreCase);
    }
}