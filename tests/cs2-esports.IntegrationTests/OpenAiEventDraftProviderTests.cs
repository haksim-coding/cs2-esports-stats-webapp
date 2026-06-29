using System.Net;
using System.Text;
using System.Text.Json;
using cs2_esports.Services.Ai;
using Microsoft.Extensions.Options;

namespace cs2_esports.IntegrationTests;

public class OpenAiEventDraftProviderTests
{
    [Fact]
    public async Task CreateDraftAsync_SendsStrictSchema_AndReadsTypedOutput()
    {
        var handler = new RecordingHandler(
            """
            {
              "output": [
                {
                  "type": "message",
                  "content": [
                    {
                      "type": "output_text",
                      "text": "{\"name\":\"IEM Zagreb\",\"organizer\":\"ESL\",\"tier\":null,\"prizePoolUsd\":250000,\"startDateUtc\":\"2026-07-15T00:00:00Z\",\"endDateUtc\":null,\"isLan\":null,\"eventVenueId\":null}"
                    }
                  ]
                }
              ]
            }
            """);
        var provider = new OpenAiEventDraftProvider(
            new HttpClient(handler),
            Options.Create(new AiProviderOptions
            {
                ApiKey = "test-key",
                Model = "test-model"
            }));

        var draft = await provider.CreateDraftAsync(
            "Create IEM Zagreb",
            new AiEventDraftContext(DateTime.UtcNow.Date, "ESL", []));

        Assert.Equal("IEM Zagreb", draft.Name);
        Assert.Equal(250000m, draft.PrizePoolUsd);
        using var request = JsonDocument.Parse(handler.RequestBody!);
        Assert.Equal("test-model", request.RootElement.GetProperty("model").GetString());
        Assert.Equal(500, request.RootElement.GetProperty("max_output_tokens").GetInt32());
        var format = request.RootElement.GetProperty("text").GetProperty("format");
        Assert.Equal("json_schema", format.GetProperty("type").GetString());
        Assert.True(format.GetProperty("strict").GetBoolean());
        Assert.False(format.GetProperty("schema").GetProperty("additionalProperties").GetBoolean());
    }

    [Fact]
    public async Task CreateDraftAsync_ExplainsExhaustedQuota()
    {
        var handler = new RecordingHandler(
            """
            {
              "error": {
                "message": "You exceeded your current quota.",
                "type": "insufficient_quota",
                "code": "insufficient_quota"
              }
            }
            """,
            HttpStatusCode.TooManyRequests);
        var provider = CreateProvider(handler);

        var exception = await Assert.ThrowsAsync<AiProviderException>(() =>
            provider.CreateDraftAsync("Create IEM Zagreb", EmptyContext()));

        Assert.Contains("quota is exhausted", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task CreateDraftAsync_ExplainsTemporaryRateLimit()
    {
        var handler = new RecordingHandler(
            """
            { "error": { "type": "rate_limit_error", "code": "rate_limit_exceeded" } }
            """,
            HttpStatusCode.TooManyRequests);
        handler.RetryAfter = TimeSpan.FromSeconds(7);
        var provider = CreateProvider(handler);

        var exception = await Assert.ThrowsAsync<AiProviderException>(() =>
            provider.CreateDraftAsync("Create IEM Zagreb", EmptyContext()));

        Assert.Contains("7 seconds", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    private static OpenAiEventDraftProvider CreateProvider(RecordingHandler handler) =>
        new(
            new HttpClient(handler),
            Options.Create(new AiProviderOptions
            {
                ApiKey = "test-key",
                Model = "test-model"
            }));

    private static AiEventDraftContext EmptyContext() =>
        new(DateTime.UtcNow.Date, null, []);

    private sealed class RecordingHandler(
        string responseBody,
        HttpStatusCode statusCode = HttpStatusCode.OK) : HttpMessageHandler
    {
        public string? RequestBody { get; private set; }
        public TimeSpan? RetryAfter { get; set; }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            RequestBody = await request.Content!.ReadAsStringAsync(cancellationToken);
            var response = new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(responseBody, Encoding.UTF8, "application/json")
            };
            if (RetryAfter.HasValue)
            {
                response.Headers.RetryAfter = new System.Net.Http.Headers.RetryConditionHeaderValue(RetryAfter.Value);
            }

            return response;
        }
    }
}
