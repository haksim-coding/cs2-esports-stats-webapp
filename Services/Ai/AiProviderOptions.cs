namespace cs2_esports.Services.Ai;

public sealed class AiProviderOptions
{
    public const string SectionName = "AiProvider";

    public string Provider { get; set; } = "Gemini";

    public string Endpoint { get; set; } = "https://generativelanguage.googleapis.com/v1beta/interactions";

    public string Model { get; set; } = "gemini-3.5-flash";

    public string ApiKey { get; set; } = string.Empty;
}
