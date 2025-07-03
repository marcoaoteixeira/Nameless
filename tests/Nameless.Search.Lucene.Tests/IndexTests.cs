using System.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using Nameless.Testing.Tools;
using Nameless.Testing.Tools.Mockers;

namespace Nameless.Search.Lucene;

public class IndexTests {
    private static readonly string IndexDirectoryPath = typeof(IndexTests).Assembly.GetDirectoryPath("Output");

    public IndexTests(ITestOutputHelper output) {
        try { Directory.Delete(IndexDirectoryPath, true); }
        catch (Exception ex) { output.WriteLine(ex.Message); }
    }

    private static ServiceProvider CreateServiceProvider() {
        var loggerFactory = new LoggerFactoryMocker()
                           .WithCreateLogger(new LoggerMocker<IndexManager>().WithAnyLogLevel().Build())
                           .WithCreateLogger(new LoggerMocker<Index>().WithAnyLogLevel().Build())
                           .Build();

        var services = new ServiceCollection();

        services.AddSingleton(loggerFactory);
        services.AddSingleton(new LoggerMocker<Index>().Build());
        services.AddSingleton(new ApplicationContextMocker().WithAppDataFolderPath(IndexDirectoryPath)
                                                            .Build());

        services.ConfigureSearchServices(_ => { });

        return services.BuildServiceProvider();
    }

    [Category(Categories.RUNS_ON_DEV_MACHINE)]
    [Fact]
    public void WhenCreatingIndex_WithIndexName_ThenReturnsIndexInstance_Cache() {
        using var provider = CreateServiceProvider();
        const string IndexName = "32b52ab3-7069-4fae-ac15-d9f3442db19b";

        var indexManager = provider.GetRequiredService<IIndexManager>();
        var indexA = indexManager.CreateIndex(IndexName);
        var indexB = indexManager.CreateIndex(IndexName);

        Assert.Multiple(() => {
            Assert.NotNull(indexA);
            Assert.NotNull(indexB);
            Assert.Same(indexA, indexB);
        });
    }

    [Category(Categories.RUNS_ON_DEV_MACHINE)]
    [Fact]
    public async Task StoreDocument_Should_Create_A_New_Document_In_Index() {
        await using var provider = CreateServiceProvider();
        const string IndexName = "d39ff2d3-7d84-4d41-99e6-e096754d14be";

        var indexManager = provider.GetRequiredService<IIndexManager>();
        var index = indexManager.CreateIndex(IndexName);

        var loremIpsumFilePath = typeof(IndexTests).Assembly.GetDirectoryPath("Resources", "LoremIpsum.txt");
        var loremIpsum = await File.ReadAllTextAsync(loremIpsumFilePath, CancellationToken.None);

        var document = new Document("146ef344-ae25-4346-b07a-7da8f418a26f")
                      .Set("Name", "Test User")
                      .Set("Email", "test_user@test.com", FieldOptions.Store)
                      .Set("Birthday", DateTime.Now.Date, FieldOptions.Store)
                      .Set("Weight", 75d, FieldOptions.Store)
                      .Set("Married", true, FieldOptions.Store)
                      .Set("Age", 50, FieldOptions.Store)
                      .Set("Content", loremIpsum, FieldOptions.Analyze | FieldOptions.Store);

        var result = await index.StoreDocumentsAsync([document], CancellationToken.None);

        Assert.Multiple(() => {
            Assert.NotNull(index);
            Assert.True(result.Succeeded);
            Assert.Equal(1, result.TotalDocumentsAffected);
        });
    }

    [Category(Categories.RUNS_ON_DEV_MACHINE)]
    [Fact]
    public async Task CreateSearchBuilder_Should_Return_Search_Service_And_Find_Document() {
        await using var provider = CreateServiceProvider();
        const string IndexName = "82b3dcd7-85c1-4c73-8f49-c54ae82ab2f8";

        var indexManager = provider.GetRequiredService<IIndexManager>();
        var index = indexManager.CreateIndex(IndexName);

        var loremIpsumFilePath = typeof(IndexTests).Assembly.GetDirectoryPath("Resources", "LoremIpsum.txt");
        var loremIpsum = await File.ReadAllTextAsync(loremIpsumFilePath, CancellationToken.None);

        var document = new Document("146ef344-ae25-4346-b07a-7da8f418a26f")
                      .Set("Name", "Test User")
                      .Set("Email", "test_user@test.com", FieldOptions.Store)
                      .Set("Birthday", DateTime.Now.Date, FieldOptions.Store)
                      .Set("Weight", 75d, FieldOptions.Store)
                      .Set("Married", true, FieldOptions.Store)
                      .Set("Age", 50, FieldOptions.Store)
                      .Set("Content", loremIpsum, FieldOptions.Analyze | FieldOptions.Store);

        await index.StoreDocumentsAsync([document], CancellationToken.None);

        var searcher = index.CreateSearchBuilder();
        var tokens = new[] { "ullamcorper", "ultrices", "Morbi", "Sbrubles" };

        searcher
           .WithField("Content", tokens[0], false)
           .Mandatory();
        searcher
           .WithField("Content", tokens[1], false)
           .Mandatory();
        searcher
           .WithField("Content", tokens[2], false)
           .ExactMatch();
        searcher
           .WithField("Content", tokens[3], false)
           .ExactMatch();

        searcher
           .WithField("Content", "and", false)
           .ExactMatch();

        var result = searcher.Search();

        Assert.Multiple(() => {
            Assert.NotNull(index);
            Assert.NotEmpty(result);
        });
    }

    [Category(Categories.RUNS_ON_DEV_MACHINE)]
    [Fact]
    public async Task Multiple_Documents_Store_Different_Moments() {
        // setup
        await using var provider = CreateServiceProvider();
        const string IndexName = "e112b156-ecfc-4fb9-90db-9675bc61b3ba";
        const string FieldName = "Content";

        // arrange
        var indexManager = provider.GetRequiredService<IIndexManager>();
        var index = indexManager.CreateIndex(IndexName);

        // act 1
        var filePathForText001 = typeof(IndexTests).Assembly.GetDirectoryPath("Resources", "text_001.txt");
        var contentText001 = await File.ReadAllTextAsync(filePathForText001, CancellationToken.None);

        var documentText001 = index
                             .NewDocument(Guid.NewGuid()
                                              .ToString("N"))
                             .Set(FieldName, contentText001, FieldOptions.Analyze | FieldOptions.Store);
        var resultDocumentText001 = await index.StoreDocumentsAsync([documentText001], CancellationToken.None);

        var searchBuilderText001 = index.CreateSearchBuilder();

        searchBuilderText001.WithField(FieldName, "vibrant", false);

        var resultText001 = searchBuilderText001
                           .Search()
                           .Select(hit => hit.GetString(FieldName))
                           .ToArray();

        // act 2
        var filePathForText002 = typeof(IndexTests).Assembly.GetDirectoryPath("Resources", "text_002.txt");
        var contentText002 = await File.ReadAllTextAsync(filePathForText002, CancellationToken.None);

        var documentText002 = index
                             .NewDocument(Guid.NewGuid()
                                              .ToString("N"))
                             .Set(FieldName, contentText002, FieldOptions.Analyze | FieldOptions.Store);
        var resultDocumentText002 = await index.StoreDocumentsAsync([documentText002], CancellationToken.None);

        var searchBuilderText002 = index.CreateSearchBuilder();

        searchBuilderText002.WithField(FieldName, "ideas", false);

        var resultText002 = searchBuilderText002
                           .Search()
                           .Select(hit => hit.GetString(FieldName))
                           .ToArray();

        Assert.Multiple(() => {
            Assert.NotEmpty(resultText001);
            Assert.True(resultDocumentText001.Succeeded);

            Assert.NotEmpty(resultText002);
            Assert.True(resultDocumentText002.Succeeded);
        });
    }
}