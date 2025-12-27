using System.Net.Http.Json;
using System.Text.Json.Serialization;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;

namespace Nameless.Mailing.MailKit;

public class MessageSummary {
    [JsonPropertyName(name: "id")]
    public Guid ID { get; set; }

    [JsonPropertyName(name: "from")]
    public string From { get; set; }

    [JsonPropertyName(name: "to")]
    public string[] To { get; set; }

    [JsonPropertyName(name: "subject")]
    public string Subject { get; set; }

    [JsonPropertyName(name: "receivedDate")]
    public DateTime ReceivedDate { get; set; }
}

public sealed class Smtp4DevContainer : IAsyncLifetime {
    private const int CONTAINER_SMTP_PORT = 25;
    private const int CONTAINER_WEB_PORT = 80;

    public const int SMTP_PORT = 2525;
    public const int WEB_PORT = 8080;

    private readonly IContainer _container = new ContainerBuilder().WithImage(image: "rnwood/smtp4dev")
                                                                   .WithName(name: "smtp4dev-test-container")
                                                                   .WithPortBinding(SMTP_PORT,
                                                                       CONTAINER_SMTP_PORT) // SMTP
                                                                   .WithPortBinding(WEB_PORT,
                                                                       CONTAINER_WEB_PORT) // Web UI/API
                                                                   .WithWaitStrategy(Wait.ForUnixContainer().UntilInternalTcpPortIsAvailable(CONTAINER_WEB_PORT))
                                                                   .WithCleanUp(cleanUp: true)
                                                                   .Build();

    private readonly HttpClient _httpClient = new() { BaseAddress = new Uri($"http://localhost:{WEB_PORT}/api/") };

    public async ValueTask InitializeAsync() {
        await _container.StartAsync();
    }

    public async ValueTask DisposeAsync() {
        await _container.DisposeAsync();
    }

    public IAsyncEnumerable<MessageSummary> GetNewestAsync(Guid? from = null, int quantity = 50) {
        return _httpClient.GetFromJsonAsAsyncEnumerable<MessageSummary>(
            $"Messages/new?lastSeenMessageId={from}&pageSize={quantity}");
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