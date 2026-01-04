using Microsoft.Extensions.DependencyInjection;
using Nameless.IO.FileSystem;
using Nameless.Lucene.Requests;
using Nameless.Testing.Tools;
using Nameless.Testing.Tools.Attributes;

namespace Nameless.Lucene;

[UnitTest]
public class IndexTests {
    private static IEnumerable<Document> CreateDocuments() {
        var files = new[] {
            "LoremIpsum.txt",
            "text_001.txt",
            "text_002.txt",
            "text_003.txt",
            "text_004.txt",
            "text_005.txt",
            "text_006.txt",
            "text_007.txt",
        };

        foreach (var file in files) {
            var content = ResourcesHelper.GetStream(file).GetContentAsString();
            yield return new Document(Guid.CreateVersion7().ToString("N"))
                .Set("content", content, FieldOptions.Analyze | FieldOptions.Store);
        }
    }

    private static IEnumerable<Document> CreateUniqueDocuments(DateTime? date = null) {
        var files = new[] {
            "unique_001.txt",
            "unique_002.txt",
            "unique_003.txt",
            "unique_004.txt",
            "unique_005.txt",
        };

        var innerDate = (date ?? DateTime.UtcNow).Date;

        foreach (var file in files) {
            var content = ResourcesHelper.GetStream(file).GetContentAsString();

            yield return new Document(Guid.CreateVersion7().ToString("N"))
                         .Set("group", file.Split('_')[0], FieldOptions.Analyze | FieldOptions.Store)
                         .Set("number", file.Split('_')[1].Replace(".txt", string.Empty), FieldOptions.Analyze | FieldOptions.Store)
                         .Set("date", innerDate, FieldOptions.Analyze | FieldOptions.Store)
                         .Set("content", content, FieldOptions.Analyze | FieldOptions.Store);

            innerDate = innerDate.AddDays(1);
        }
    }

    [Fact]
    public async Task Happy_Path() {
        const string IndexName = "bc8a58ca6a3b41558ccd30c5adaccd9c";

        await using var provider = ServicesHelper.CreateContainer();

        // cleanup previous index if any
        var fileSystem = provider.GetRequiredService<IFileSystem>();
        var indexDirectory = fileSystem.GetDirectory(Path.Combine(ServicesHelper.IndexesDirectoryName, IndexName));
        if (indexDirectory.Exists) {
            indexDirectory.Delete(recursive: true);
        }

        var indexProvider = provider.GetRequiredService<IIndexProvider>();
        using var index = indexProvider.Get(IndexName);
        Assert.NotNull(index);

        var documents = CreateDocuments().ToArray();

        // store documents
        var request = new InsertDocumentsRequest(documents);
        var response = await index.InsertDocumentsAsync(request, CancellationToken.None);
        Assert.Multiple(() => {
            Assert.True(response.Success);
            Assert.Equal(documents.Length, response.Value.TotalDocumentsInserted);
        });

        // retrieve documents
        var queryDefinition = index.CreateQueryBuilder()
                                   .Build(); // match all query

        var searchRequest = new SearchDocumentsRequest(queryDefinition.Query);
        var searchResponse = await index.SearchDocumentsAsync(searchRequest, CancellationToken.None);
        Assert.Multiple(() => {
            Assert.True(searchResponse.Success);
            Assert.Equal(8, searchResponse.Value.Count);
        });

        // delete documents
        var deleteRequest = new DeleteDocumentsByQueryRequest(queryDefinition.Query);
        var deleteResponse = await index.DeleteDocumentsAsync(deleteRequest, CancellationToken.None);
        Assert.Multiple(() => {
            Assert.True(deleteResponse.Success);
            Assert.Equal(8, deleteResponse.Value.TotalDocumentsDeleted);
        });

        // ensure index is empty
        var emptySearchRequest = new SearchDocumentsRequest(queryDefinition.Query);
        var emptySearchResponse = await index.SearchDocumentsAsync(emptySearchRequest, CancellationToken.None);
        Assert.Multiple(() => {
            Assert.True(emptySearchResponse.Success);
            Assert.Empty(emptySearchResponse.Value.Hits);
        });
    }

    [Fact]
    public async Task WhenSearch_WhenUsingField_ThenReturnsDocumentsWithTerm() {
        const string IndexName = "dc237104-e52f-48fc-92a2-f071a999c8fd";

        await using var provider = ServicesHelper.CreateContainer();

        // cleanup previous index if any
        var fileSystem = provider.GetRequiredService<IFileSystem>();
        var indexDirectory = fileSystem.GetDirectory(Path.Combine(ServicesHelper.IndexesDirectoryName, IndexName));
        if (indexDirectory.Exists) {
            indexDirectory.Delete(recursive: true);
        }

        var indexProvider = provider.GetRequiredService<IIndexProvider>();
        using var index = indexProvider.Get(IndexName);
        Assert.NotNull(index);

        var documents = CreateUniqueDocuments().ToArray();

        // store documents
        var insertRequest = new InsertDocumentsRequest(documents);
        var insertResponse = await index.InsertDocumentsAsync(insertRequest, CancellationToken.None);
        Assert.Multiple(() => {
            Assert.True(insertResponse.Success);
            Assert.Equal(documents.Length, insertResponse.Value.TotalDocumentsInserted);
        });

        // retrieve documents
        var queryDefinition = index.CreateQueryBuilder()
                                   .WithField("content", "corujas", useWildcard: false)
                                   .Build();

        var searchRequest = new SearchDocumentsRequest(queryDefinition.Query);
        var searchResponse = await index.SearchDocumentsAsync(searchRequest, cancellationToken: CancellationToken.None);
        Assert.Multiple(() => {
            Assert.True(searchResponse.Success);
            Assert.Single(searchResponse.Value.Hits);
        });
    }

    [Fact]
    public async Task WhenSearch_WhenUsingField_WithWildcard_ThenReturnsDocumentsWithTerm() {
        const string IndexName = "b02b8b0c-3861-4106-845d-51c4122fb321";

        await using var provider = ServicesHelper.CreateContainer();

        // cleanup previous index if any
        var fileSystem = provider.GetRequiredService<IFileSystem>();
        var indexDirectory = fileSystem.GetDirectory(Path.Combine(ServicesHelper.IndexesDirectoryName, IndexName));
        if (indexDirectory.Exists) {
            indexDirectory.Delete(recursive: true);
        }

        var indexProvider = provider.GetRequiredService<IIndexProvider>();
        using var index = indexProvider.Get(IndexName);
        Assert.NotNull(index);

        var documents = CreateUniqueDocuments().ToArray();

        // store documents
        var insertRequest = new InsertDocumentsRequest(documents);
        var insertResponse = await index.InsertDocumentsAsync(insertRequest, CancellationToken.None);
        Assert.Multiple(() => {
            Assert.True(insertResponse.Success);
            Assert.Equal(documents.Length, insertResponse.Value.TotalDocumentsInserted);
        });

        // retrieve documents
        var queryDefinition = index.CreateQueryBuilder()
                                   .WithField("content", "*am", useWildcard: true)
                                   .Build();

        var searchRequest = new SearchDocumentsRequest(queryDefinition.Query);
        var searchResponse = await index.SearchDocumentsAsync(searchRequest, CancellationToken.None);
        Assert.Multiple(() => {
            Assert.True(searchResponse.Success);
            Assert.Equal(5, searchResponse.Value.Count);
        });
    }

    [Fact]
    public async Task WhenSearch_WhenUsingDateRange_ThenReturnsDocuments() {
        const string IndexName = "b02b8b0c-3861-4106-845d-51c4122fb321";

        await using var provider = ServicesHelper.CreateContainer();

        // cleanup previous index if any
        var fileSystem = provider.GetRequiredService<IFileSystem>();
        var indexDirectory = fileSystem.GetDirectory(Path.Combine(ServicesHelper.IndexesDirectoryName, IndexName));
        if (indexDirectory.Exists) {
            indexDirectory.Delete(recursive: true);
        }

        var indexProvider = provider.GetRequiredService<IIndexProvider>();
        using var index = indexProvider.Get(IndexName);
        Assert.NotNull(index);

        var date = DateTime.UtcNow.Date;
        var documents = CreateUniqueDocuments(date).ToArray();

        // store documents
        var insertRequest = new InsertDocumentsRequest(documents);
        var insertResponse = await index.InsertDocumentsAsync(insertRequest, CancellationToken.None);
        Assert.Multiple(() => {
            Assert.True(insertResponse.Success);
            Assert.Equal(documents.Length, insertResponse.Value.TotalDocumentsInserted);
        });

        // retrieve documents
        var term = date.AddDays(2);
        var queryDefinition = index.CreateQueryBuilder()
                                   .WithinRange(
                                       name: "date",
                                       minimum: term,
                                       maximum: null,
                                       includeMinimum: true,
                                       includeMaximum: false
                                    )
                                   .Build();

        var searchRequest = new SearchDocumentsRequest(queryDefinition.Query);
        var searchResponse = await index.SearchDocumentsAsync(searchRequest, CancellationToken.None);
        Assert.Multiple(() => {
            Assert.True(searchResponse.Success);
            Assert.Equal(3, searchResponse.Value.Count);

            foreach (var result in searchResponse.Value.Hits) {
                var resultDate = result.GetDateTime("date");

                Assert.Equal(term, resultDate);

                term = term.AddDays(1);
            }
        });
    }
}
