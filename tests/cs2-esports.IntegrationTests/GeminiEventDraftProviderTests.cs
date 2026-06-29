using System.Net;
using System.Text;
using System.Text.Json;
using cs2_esports.Services.Ai;
using Microsoft.Extensions.Options;

namespace cs2_esports.IntegrationTests;

public class GeminiEventDraftProviderTests
{
    [Fact]
    public async Task CreateDraftAsync_UsesGeminiAuthenticationAndStructuredOutput()
    {
        var handler = new RecordingHandler(
            """
            {
              "status": "completed",
              "steps": [
                {
                  "type": "model_output",
                  "content": [
                    {
                      "type": "text",
                      "text": "{\"name\":\"IEM Zagreb\",\"organizer\":\"ESL\",\"tier\":null,\"prizePoolUsd\":250000,\"startDateUtc\":\"2026-07-15T00:00:00Z\",\"endDateUtc\":null,\"isLan\":null,\"eventVenueId\":null}"
                    }
                  ]
                }
              ]
            }
            """);
        var provider = CreateProvider(handler);

        var draft = await provider.CreateDraftAsync(
            "Create IEM Zagreb",
            new AiEventDraftContext(DateTime.UtcNow.Date, "ESL", []));

        Assert.Equal("IEM Zagreb", draft.Name);
        Assert.Equal(250000m, draft.PrizePoolUsd);
        Assert.Equal("test-key", handler.ApiKey);
        Assert.Null(handler.Authorization);

        using var request = JsonDocument.Parse(handler.RequestBody!);
        Assert.Equal("test-model", request.RootElement.GetProperty("model").GetString());
        Assert.False(request.RootElement.GetProperty("store").GetBoolean());
        var format = request.RootElement.GetProperty("response_format");
        Assert.Equal("application/json", format.GetProperty("mime_type").GetString());
        Assert.False(format.GetProperty("schema").GetProperty("additionalProperties").GetBoolean());
    }

    [Fact]
    public async Task CreateDraftAsync_ExplainsRejectedGeminiKey()
    {
        var handler = new RecordingHandler(
            """
            { "error": { "code": 401, "message": "API key not valid", "status": "UNAUTHENTICATED" } }
            """,
            HttpStatusCode.Unauthorized);
        var provider = CreateProvider(handler);

        var exception = await Assert.ThrowsAsync<AiProviderException>(() =>
            provider.CreateDraftAsync(
                "Create IEM Zagreb",
                new AiEventDraftContext(DateTime.UtcNow.Date, null, [])));

        Assert.Contains("Google AI Studio", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    private static GeminiEventDraftProvider CreateProvider(RecordingHandler handler) =>
        new(
            new HttpClient(handler),
            Options.Create(new AiProviderOptions
            {
                Provider = "Gemini",
                Endpoint = "https://generativelanguage.googleapis.com/v1beta/interactions",
                ApiKey = "test-key",
                Model = "test-model"
            }));

    private sealed class RecordingHandler(
        string responseBody,
        HttpStatusCode statusCode = HttpStatusCode.OK) : HttpMessageHandler
    {
        public string? RequestBody { get; private set; }
        public string? ApiKey { get; private set; }
        public string? Authorization { get; private set; }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            RequestBody = await request.Content!.ReadAsStringAsync(cancellationToken);
            ApiKey = request.Headers.TryGetValues("x-goog-api-key", out var values)
                ? values.Single()
                : null;
            Authorization = request.Headers.Authorization?.ToString();
            return new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(responseBody, Encoding.UTF8, "application/json")
            };
        }
    }
}
