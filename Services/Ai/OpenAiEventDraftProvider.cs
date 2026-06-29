using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net;
using System.Text.Json;
using cs2_esports.Dtos.Events;
using Microsoft.Extensions.Options;

namespace cs2_esports.Services.Ai;

public sealed class OpenAiEventDraftProvider : IAiEventDraftProvider
{
    private readonly HttpClient _httpClient;
    private readonly AiProviderOptions _options;

    public OpenAiEventDraftProvider(HttpClient httpClient, IOptions<AiProviderOptions> options)
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
                "AI drafting is not configured. Set OPENAI_API_KEY or AiProvider:ApiKey.");
        }

        if (!Uri.TryCreate(_options.Endpoint, UriKind.Absolute, out var endpoint))
        {
            throw new AiProviderException("The configured AI provider endpoint is invalid.");
        }

        var requestBody = new
        {
            model = _options.Model,
            max_output_tokens = 500,
            instructions = AiEventDraftContract.BuildInstructions(context),
            input = prompt,
            text = new
            {
                format = new
                {
                    type = "json_schema",
                    name = "ai_event_draft",
                    strict = true,
                    schema = AiEventDraftContract.BuildSchema()
                }
            }
        };

        using var request = new HttpRequestMessage(HttpMethod.Post, endpoint)
        {
            Content = JsonContent.Create(requestBody, options: AiEventDraftContract.JsonOptions)
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.ApiKey);

        HttpResponseMessage response;
        try
        {
            response = await _httpClient.SendAsync(request, cancellationToken);
        }
        catch (Exception exception) when (exception is HttpRequestException or TaskCanceledException)
        {
            throw new AiProviderException("The AI provider could not be reached.", exception);
        }

        using (response)
        {
            if (!response.IsSuccessStatusCode)
            {
                throw await CreateProviderExceptionAsync(response, cancellationToken);
            }

            try
            {
                await using var responseStream = await response.Content.ReadAsStreamAsync(cancellationToken);
                using var document = await JsonDocument.ParseAsync(responseStream, cancellationToken: cancellationToken);
                var outputText = ReadOutputText(document.RootElement);
                return JsonSerializer.Deserialize<AiEventDraft>(outputText, AiEventDraftContract.JsonOptions)
                    ?? throw new AiProviderException("The AI provider returned an empty event draft.");
            }
            catch (AiProviderException)
            {
                throw;
            }
            catch (Exception exception) when (exception is JsonException or InvalidOperationException)
            {
                throw new AiProviderException("The AI provider returned an invalid event draft.", exception);
            }
        }
    }

    private static async Task<AiProviderException> CreateProviderExceptionAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        string? errorCode = null;
        string? errorType = null;

        try
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            using var document = JsonDocument.Parse(body);
            if (document.RootElement.TryGetProperty("error", out var error))
            {
                errorCode = ReadString(error, "code");
                errorType = ReadString(error, "type");
            }
        }
        catch (JsonException)
        {
            // Fall back to the HTTP status when the provider did not return its documented error shape.
        }

        if (response.StatusCode == HttpStatusCode.TooManyRequests)
        {
            if (string.Equals(errorCode, "insufficient_quota", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(errorType, "insufficient_quota", StringComparison.OrdinalIgnoreCase))
            {
                return new AiProviderException(
                    "OpenAI API quota is exhausted. Add API credits or raise the project budget, then try again.");
            }

            var retryAfter = GetRetryAfter(response.Headers.RetryAfter);
            return new AiProviderException(
                retryAfter is null
                    ? "OpenAI rate limit reached. Wait briefly and try again."
                    : $"OpenAI rate limit reached. Try again in about {retryAfter.Value} seconds.");
        }

        return new AiProviderException(
            $"The AI provider returned HTTP {(int)response.StatusCode}.");
    }

    private static string? ReadString(JsonElement element, string propertyName) =>
        element.TryGetProperty(propertyName, out var property) && property.ValueKind == JsonValueKind.String
            ? property.GetString()
            : null;

    private static int? GetRetryAfter(RetryConditionHeaderValue? retryAfter)
    {
        if (retryAfter?.Delta is { } delta)
        {
            return Math.Max(1, (int)Math.Ceiling(delta.TotalSeconds));
        }

        if (retryAfter?.Date is { } date)
        {
            return Math.Max(1, (int)Math.Ceiling((date - DateTimeOffset.UtcNow).TotalSeconds));
        }

        return null;
    }

    private static string ReadOutputText(JsonElement response)
    {
        if (!response.TryGetProperty("output", out var output) || output.ValueKind != JsonValueKind.Array)
        {
            throw new AiProviderException("The AI provider response did not contain output.");
        }

        foreach (var outputItem in output.EnumerateArray())
        {
            if (!outputItem.TryGetProperty("content", out var content) || content.ValueKind != JsonValueKind.Array)
            {
                continue;
            }

            foreach (var contentItem in content.EnumerateArray())
            {
                if (contentItem.TryGetProperty("type", out var type) &&
                    type.GetString() == "output_text" &&
                    contentItem.TryGetProperty("text", out var text) &&
                    !string.IsNullOrWhiteSpace(text.GetString()))
                {
                    return text.GetString()!;
                }

                if (contentItem.TryGetProperty("type", out type) && type.GetString() == "refusal")
                {
                    throw new AiProviderException("The AI provider declined to create this draft.");
                }
            }
        }

        throw new AiProviderException("The AI provider response did not contain an event draft.");
    }
}
