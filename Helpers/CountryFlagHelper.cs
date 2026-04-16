namespace cs2_esports.Helpers;

public static class CountryFlagHelper
{
    private static readonly Dictionary<string, string> Aliases = new(StringComparer.OrdinalIgnoreCase)
    {
        ["UK"] = "GB",
        ["EN"] = "GB",
        ["EL"] = "GR"
    };

    private static readonly Dictionary<string, string> Iso3ToIso2 = new(StringComparer.OrdinalIgnoreCase)
    {
        ["DEU"] = "DE",
        ["DNK"] = "DK",
        ["FRA"] = "FR",
        ["HRV"] = "HR",
        ["POL"] = "PL",
        ["RUS"] = "RU",
        ["UKR"] = "UA"
    };

    private static readonly Dictionary<string, string> CountryNameToIso2 = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Germany"] = "DE",
        ["Denmark"] = "DK",
        ["France"] = "FR",
        ["Croatia"] = "HR",
        ["Poland"] = "PL",
        ["Russia"] = "RU",
        ["Ukraine"] = "UA",
        ["Europe"] = "EU",
        ["European Union"] = "EU"
    };

    public static string GetFlagImageUrl(string? countryCode)
    {
        var normalizedCode = NormalizeCode(countryCode);
        if (normalizedCode is null)
        {
            return "https://flagcdn.com/w40/un.png";
        }

        return $"https://flagcdn.com/w40/{normalizedCode.ToLowerInvariant()}.png";
    }

    private static string? NormalizeCode(string? countryCode)
    {
        if (string.IsNullOrWhiteSpace(countryCode))
        {
            return null;
        }

        var trimmed = countryCode.Trim();
        if (CountryNameToIso2.TryGetValue(trimmed, out var nameCode))
        {
            return nameCode;
        }

        var code = trimmed.ToUpperInvariant();

        if (Aliases.TryGetValue(code, out var aliasCode))
        {
            return aliasCode;
        }

        if (Iso3ToIso2.TryGetValue(code, out var iso2Code))
        {
            return iso2Code;
        }

        if (code.Length == 2 && code.All(character => character >= 'A' && character <= 'Z'))
        {
            return code;
        }

        return null;
    }
}