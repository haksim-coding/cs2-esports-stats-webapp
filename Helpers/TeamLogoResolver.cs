namespace cs2_esports.Helpers;

public static class TeamLogoResolver
{
    public static string GetBadgeText(string? teamName, string? teamTag)
    {
        var source = string.IsNullOrWhiteSpace(teamTag) ? teamName : teamTag;

        if (string.IsNullOrWhiteSpace(source))
        {
            return "TEAM";
        }

        var normalized = Normalize(source);
        if (normalized.Length <= 3)
        {
            return normalized.ToUpperInvariant();
        }

        return normalized[..3].ToUpperInvariant();
    }

    public static string? GetLogoPath(string? teamName, string? teamTag)
    {
        foreach (var candidate in new[] { teamName, teamTag }.Where(value => !string.IsNullOrWhiteSpace(value)))
        {
            var normalized = Normalize(candidate!);

            switch (normalized)
            {
                case "vitality":
                case "teamvitality":
                    return "~/images/teams/Vitality.svg";
                case "navi":
                case "natusvincere":
                    return "~/images/teams/NAVI.svg";
                case "faze":
                case "fazeclan":
                    return "~/images/teams/FaZe.svg";
                case "g2":
                case "g2esports":
                    return "~/images/teams/G2.svg";
                case "mouz":
                case "mousesports":
                    return "~/images/teams/MOUZ.svg";
                case "spirit":
                case "teamspirit":
                    return "~/images/teams/Spirit.svg";
            }
        }

        return null;
    }

    private static string Normalize(string value)
    {
        var builder = new System.Text.StringBuilder(value.Length);

        foreach (var character in value.Trim().ToLowerInvariant())
        {
            if (char.IsLetterOrDigit(character))
            {
                builder.Append(character);
            }
        }

        return builder.ToString();
    }
}