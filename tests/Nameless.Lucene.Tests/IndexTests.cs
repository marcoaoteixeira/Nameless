using Microsoft.Extensions.DependencyInjection;
using Nameless.IO.FileSystem;
using Nameless.Testing.Tools;
using Nameless.Testing.Tools.Attributes;

namespace Nameless.Lucene;

[UnitTest]
public class IndexTests {
    private static IEnumerable<IDocument> CreateDocuments() {
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
        var insertResult = await index.InsertAsync(documents, CancellationToken.None);
        Assert.Multiple(() => {
            Assert.True(insertResult.Succeeded);
            Assert.Equal(documents.Length, insertResult.Count);
        });

        // retrieve documents
        var queryDefinition = index.CreateQueryBuilder()
                                   .Build(); // match all query

        var searchResult = await index.SearchAsync(queryDefinition.Query, cancellationToken: CancellationToken.None);
        Assert.Multiple(() => {
            Assert.True(searchResult.Succeeded);
            Assert.Equal(8, searchResult.Results.Length);
        });

        // delete documents
        var removeResult = await index.RemoveAsync(queryDefinition.Query, CancellationToken.None);
        Assert.Multiple(() => {
            Assert.True(removeResult.Succeeded);
            Assert.Equal(8, removeResult.Count);
        });

        // ensure index is empty
        var emptySearchResult = await index.SearchAsync(queryDefinition.Query, cancellationToken: CancellationToken.None);
        Assert.Multiple(() => {
            Assert.True(emptySearchResult.Succeeded);
            Assert.Empty(emptySearchResult.Results);
        });
    }
}
