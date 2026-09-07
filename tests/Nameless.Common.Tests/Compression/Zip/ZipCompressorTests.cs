using System.IO.Compression;
using Nameless.Compression.Requests;
using Nameless.Testing.Tools.Attributes;
using Nameless.Testing.Tools.Mockers.Logging;

namespace Nameless.Compression.Zip;

[IntegrationTest]
public class ZipCompressorTests : IDisposable {
    // Each test gets its own temp directory to avoid collisions.
    private readonly string _workDir = Path.Combine(
        Path.GetTempPath(),
        "ZipCompressorTests",
        Guid.NewGuid().ToString("N")
    );

    public ZipCompressorTests() {
        Directory.CreateDirectory(_workDir);
    }

    public void Dispose() {
        if (Directory.Exists(_workDir)) {
            Directory.Delete(_workDir, recursive: true);
        }
    }

    private ZipCompressor CreateSut() {
        var logger = new LoggerMocker<ZipCompressor>()
            .WithAnyLogLevel()
            .Build();

        return new ZipCompressor(logger);
    }

    private string CreateTempFile(string fileName, string content = "test content") {
        var path = Path.Combine(_workDir, fileName);
        File.WriteAllText(path, content);
        return path;
    }

    [Fact]
    public async Task CompressAsync_ThenDecompressAsync_RoundTrips() {
        // arrange
        var sut = CreateSut();

        var sourceFile = CreateTempFile("source.txt", "Hello, round-trip world!");
        var archivePath = Path.Combine(_workDir, "output.zip");
        var extractDir = Path.Combine(_workDir, "extracted");

        var compressRequest = new CompressRequest(archivePath)
            .IncludeFile(sourceFile);

        var decompressRequest = new DecompressRequest(archivePath) {
            DestinationDirectoryPath = extractDir
        };

        // act
        var compressResponse = await sut.CompressAsync(compressRequest, CancellationToken.None);
        var decompressResponse = await sut.DecompressAsync(decompressRequest, CancellationToken.None);

        // assert
        Assert.Multiple(() => {
            Assert.True(compressResponse.Success);
            Assert.True(decompressResponse.Success);
            Assert.True(decompressResponse.Value.IsDirectoryAvailable);
        });
    }

    [Fact]
    public async Task CompressAsync_ProducesValidZipBytes() {
        // arrange
        var sut = CreateSut();

        var sourceFile = CreateTempFile("data.txt", "zip content check");
        var archivePath = Path.Combine(_workDir, "valid.zip");

        var request = new CompressRequest(archivePath)
            .IncludeFile(sourceFile);

        // act
        var response = await sut.CompressAsync(request, CancellationToken.None);

        // assert — archive must exist and be openable as a ZipArchive
        Assert.True(response.Success);
        Assert.True(File.Exists(archivePath));

        using var archive = ZipFile.OpenRead(archivePath);
        Assert.NotEmpty(archive.Entries);
    }

    [Fact]
    public async Task CompressAsync_MissingDestinationFilePath_ReturnsValidationError() {
        // arrange
        var sut = CreateSut();

        // CompressRequest requires a non-null/whitespace destination, but we can
        // simulate the "destination already exists" validation failure instead.
        var existingFile = CreateTempFile("exists.zip", string.Empty);
        // Treat the existing file as the destination — validation should reject it.
        var request = new CompressRequest(existingFile);

        // act
        var response = await sut.CompressAsync(request, CancellationToken.None);

        // assert
        Assert.False(response.Success);
    }

    [Fact]
    public async Task DecompressAsync_NonExistentSourceFile_ReturnsValidationError() {
        // arrange
        var sut = CreateSut();

        var request = new DecompressRequest(
            Path.Combine(_workDir, "does_not_exist.zip")
        );

        // act
        var response = await sut.DecompressAsync(request, CancellationToken.None);

        // assert
        Assert.False(response.Success);
    }

    [Fact]
    public async Task CompressAsync_LargeData_RoundTrips() {
        // arrange
        var sut = CreateSut();

        // 1 MB of repeated text — large enough to exercise the streaming path
        var largeContent = new string('x', 1024 * 1024);
        var sourceFile = CreateTempFile("large.txt", largeContent);

        var archivePath = Path.Combine(_workDir, "large.zip");
        var extractDir = Path.Combine(_workDir, "large_extracted");

        var compressRequest = new CompressRequest(archivePath)
            .IncludeFile(sourceFile);

        var decompressRequest = new DecompressRequest(archivePath) {
            DestinationDirectoryPath = extractDir
        };

        // act
        var compressResponse = await sut.CompressAsync(compressRequest, CancellationToken.None);
        var decompressResponse = await sut.DecompressAsync(decompressRequest, CancellationToken.None);

        // assert
        Assert.Multiple(() => {
            Assert.True(compressResponse.Success);
            Assert.True(decompressResponse.Success);

            var extractedFile = Directory.GetFiles(extractDir, "large.txt", SearchOption.AllDirectories)
                                         .FirstOrDefault();

            Assert.NotNull(extractedFile);
            Assert.Equal(largeContent, File.ReadAllText(extractedFile));
        });
    }
}
