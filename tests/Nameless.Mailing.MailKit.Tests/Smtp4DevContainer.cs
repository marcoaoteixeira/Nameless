using System.Net.Http.Json;
using System.Text.Json.Serialization;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;

namespace Nameless.Mailing.MailKit;

public class MessageSummary {
    [JsonPropertyName("id")]
    public Guid ID { get; set; }

    [JsonPropertyName("from")]
    public string From { get; set; }

    [JsonPropertyName("to")]
    public string[] To { get; set; }

    [JsonPropertyName("subject")]
    public string Subject { get; set; }

    [JsonPropertyName("receivedDate")]
    public DateTime ReceivedDate { get; set; }
}

public sealed class Smtp4DevContainer : IAsyncLifetime {
    public const int SMTP_PORT = 2525;
    private const int WEB_UI_PORT = 8080;

    private readonly IContainer _container = new ContainerBuilder().WithImage("rnwood/smtp4dev")
                                                                   .WithPortBinding(SMTP_PORT, 25) // SMTP
                                                                   .WithPortBinding(WEB_UI_PORT, 80) // Web UI/API
                                                                   .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(80))
                                                                   .WithName("smtp4dev-test")
                                                                   .Build();
    private readonly HttpClient _httpClient = new() {
        BaseAddress = new Uri($"http://localhost:{WEB_UI_PORT}/api/")
    };

    public async Task InitializeAsync() {
        await _container.StartAsync();
    }

    public async Task DisposeAsync() {
        await _container.DisposeAsync();
    }

    public IAsyncEnumerable<MessageSummary> GetNewestAsync(Guid? from = null, int quantity = 50) {
        return _httpClient.GetFromJsonAsAsyncEnumerable<MessageSummary>($"Messages/new?lastSeenMessageId={from}&pageSize={quantity}");
    }

    public Task<string> GetMessageContentAsync(Guid id, bool isHtml = false) {
        return _httpClient.GetStringAsync($"Messages/{id}/{(isHtml ? "html" : "plaintext")}");
    }
}

// This class has no code, and is never created. Its purpose is simply to be
// the place to apply [CollectionDefinition] and all the ICollectionFixture<> interfaces.
// See https://xunit.net/docs/shared-context for more information.
[CollectionDefinition(nameof(Smtp4DevContainerCollection))]
public sealed class Smtp4DevContainerCollection : ICollectionFixture<Smtp4DevContainer>;