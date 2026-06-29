using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using cs2_esports.Dtos.Events;
using Microsoft.Extensions.Options;

namespace cs2_esports.Services.Ai;

public sealed class GeminiEventDraftProvider : IAiEventDraftProvider
{
    private readonly HttpClient _httpClient;
    private readonly AiProviderOptions _options;

    public GeminiEventDraftProvider(HttpClient httpClient, IOptions<AiProviderOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task<AiEventDraft> CreateDraftAsync(
        string prompt,
        AiEventDraftContext context,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_options.ApiKey))
        {
            throw new AiProviderException(
                "Gemini drafting is not configured. Set GEMINI_API_KEY or AiProvider:ApiKey.");
        }

        if (!Uri.TryCreate(_options.Endpoint, UriKind.Absolute, out var endpoint))
        {
            throw new AiProviderException("The configured Gemini endpoint is invalid.");
        }

        var requestBody = new
        {
            model = _options.Model,
            input = prompt,
            system_instruction = AiEventDraftContract.BuildInstructions(context),
            store = false,
            response_format = new
            {
                type = "text",
                mime_type = "application/json",
                schema = AiEventDraftContract.BuildSchema()
            }
        };

        using var request = new HttpRequestMessage(HttpMethod.Post, endpoint)
        {
            Content = JsonContent.Create(requestBody, options: AiEventDraftContract.JsonOptions)
        };
        request.Headers.Add("x-goog-api-key", _options.ApiKey);
        request.Headers.Add("Api-Revision", "2026-05-20");

        HttpResponseMessage response;
        try
        {
            response = await _httpClient.SendAsync(request, cancellationToken);
        }
        catch (Exception exception) when (exception is HttpRequestException or TaskCanceledException)
        {
            throw new AiProviderException("Gemini could not be reached.", exception);
        }

        using (response)
        {
            if (!response.IsSuccessStatusCode)
            {
                throw await CreateProviderExceptionAsync(response, cancellationToken);
            }

            try
            {
                await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
                using var document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);
                var outputText = ReadOutputText(document.RootElement);
                return JsonSerializer.Deserialize<AiEventDraft>(outputText, AiEventDraftContract.JsonOptions)
                    ?? throw new AiProviderException("Gemini returned an empty event draft.");
            }
            catch (AiProviderException)
            {
                throw;
            }
            catch (Exception exception) when (exception is JsonException or InvalidOperationException)
            {
                throw new AiProviderException("Gemini returned an invalid event draft.", exception);
            }
        }
    }

    private static string ReadOutputText(JsonElement response)
    {
        if (!response.TryGetProperty("steps", out var steps) || steps.ValueKind != JsonValueKind.Array)
        {
            throw new AiProviderException("Gemini response did not contain output steps.");
        }

        foreach (var step in steps.EnumerateArray().Reverse())
        {
            if (!step.TryGetProperty("type", out var stepType) || stepType.GetString() != "model_output" ||
                !step.TryGetProperty("content", out var content) || content.ValueKind != JsonValueKind.Array)
            {
                continue;
            }

            var textParts = content.EnumerateArray()
                .Where(item => item.TryGetProperty("type", out var type) && type.GetString() == "text")
                .Select(item => item.TryGetProperty("text", out var text) ? text.GetString() : null)
                .Where(text => !string.IsNullOrWhiteSpace(text));
            var outputText = string.Concat(textParts);
            if (!string.IsNullOrWhiteSpace(outputText))
            {
                return outputText;
            }
        }

        throw new AiProviderException("Gemini response did not contain an event draft.");
    }

    private static async Task<AiProviderException> CreateProviderExceptionAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        string? providerMessage = null;
        try
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            using var document = JsonDocument.Parse(body);
            if (document.RootElement.TryGetProperty("error", out var error) &&
                error.TryGetProperty("message", out var message) &&
                message.ValueKind == JsonValueKind.String)
            {
                providerMessage = message.GetString();
            }
        }
        catch (JsonException)
        {
            // Fall back to a status-specific message.
        }

        return response.StatusCode switch
        {
            HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden => new AiProviderException(
                "Gemini rejected the API key. Use a Gemini API key from Google AI Studio and check its API restrictions."),
            HttpStatusCode.TooManyRequests => new AiProviderException(
                "Gemini rate limit or free-tier quota reached. Wait for the quota to reset or check the Google AI Studio usage page."),
            HttpStatusCode.BadRequest when !string.IsNullOrWhiteSpace(providerMessage) => new AiProviderException(
                $"Gemini rejected the request: {providerMessage}"),
            _ => new AiProviderException($"Gemini returned HTTP {(int)response.StatusCode}.")
        };
    }
}
