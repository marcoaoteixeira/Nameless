using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using Nameless.Testing.Tools.Attributes;
using Nameless.Testing.Tools.Mockers.Http;

namespace Nameless.Common.Testing.Tools.Mockers.Examples;

[UnitTest]
public class HttpMessageHandlerMockerExamples
{
    [Fact]
    public async Task Using_HttpMessageHandlerMocker_WithHttpClient()
    {
        const string Message = "It works!";
        var obj = new { Message };
        var json = JsonSerializer.Serialize(obj);

        var httpMessageHandlerMocker = new HttpMessageHandlerMocker();

        httpMessageHandlerMocker.WithSendAsync(obj);

        var httpMessageHandler = httpMessageHandlerMocker.Build();
        // ReSharper disable once ShortLivedHttpClient
        using var httpClient = new HttpClient(httpMessageHandler);

        var response = await httpClient.GetAsync("https://www.enhesa.com/", TestContext.Current.CancellationToken);
        var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        Assert.Equal(json, content);
    }

    [Fact]
    public async Task Using_HttpMessageHandlerMocker_WithHttpClient_WhenUsingHttpClientFactory()
    {
        // Suppose you're using WebApplicationFactory to run automated tests
        // for your API. The factory allows you to override service
        // registrations within the application's ServiceCollection.
        // To do this, override the WebApplicationFactory.ConfigureWebHost
        // method and use the "builder" parameter with ConfigureServices
        // method to inject your mocked HttpMessageHandler.

        const string Message = "It works...a second time!";
        var obj = new { Message };
        var json = JsonSerializer.Serialize(obj);
        var httpMessageHandler = new HttpMessageHandlerMocker()
            .WithSendAsync(obj)
            .Build();

        var services = new ServiceCollection();

        // Configure your client as usual
        services.AddHttpClient<IHttpContentService, HttpContentService>(
            config => config.BaseAddress = new Uri("https://www.enhesa.com")
        );
        
        // If using WebApplicationFactory, on ConfigureWebHost method
        // call builder.ConfigureServices(services => ...) and add
        // your HttpMessageHandler override.
        services.Configure<HttpClientFactoryOptions>(
            nameof(IHttpContentService),
            opts => opts.HttpMessageHandlerBuilderActions.Add(
                builder => builder.PrimaryHandler = httpMessageHandler
            )
        );

        var provider = services.BuildServiceProvider();
        var httpContentService = provider.GetRequiredService<IHttpContentService>();
        var content = await httpContentService.GetAsync("endpoint", TestContext.Current.CancellationToken);

        Assert.Equal(json, content);
    }

    public interface IHttpContentService
    {
        Task<string> GetAsync(string url, CancellationToken cancellationToken);
    }

    public class HttpContentService : IHttpContentService
    {
        private readonly HttpClient _client;

        public HttpContentService(HttpClient client)
        {
            _client = client;
        }

        public async Task<string> GetAsync(string url, CancellationToken cancellationToken)
        {
            var response = await _client.GetAsync(url, cancellationToken);

            return await response.Content.ReadAsStringAsync(cancellationToken);
        }
    }
}
