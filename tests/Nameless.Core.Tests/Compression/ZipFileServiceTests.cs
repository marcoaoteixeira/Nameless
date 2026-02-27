using System.IO.Compression;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Nameless.Compression.Requests;
using Nameless.Testing.Tools.Attributes;
using Nameless.Testing.Tools.Resources;

namespace Nameless.Compression;

[UnitTest]
public class ZipFileServiceTests {
    private const string RESOURCE_FILE = "LoremIpsum.txt";

    private static ZipFileService CreateSut(ILogger<ZipFileService> logger = null) {
        logger ??= NullLogger<ZipFileService>.Instance;

        return new ZipFileService(logger);
    }

    [Fact]
    public async Task WhenCreatingCompressFile_WithValidParameters_ThenGenerateCompressedFile() {
        // arrange
        var sut = CreateSut();

        await using var sourceFilePath = ResourcesHelper.GetResource(RESOURCE_FILE);
        var destinationFilePath = Path.Combine(Path.GetTempPath(), $"{Guid.CreateVersion7():N}.zip");
        var request = new CompressRequest(destinationFilePath) {
            CompressionLevel = CompressionLevel.Optimal
        }.IncludeFile(sourceFilePath.Path, "temp/bar/fuzz");

        // act
        var result = await sut.CompressAsync(request, TestContext.Current.CancellationToken);

        // assert
        result.Match(
            onSuccess: value => {
                Assert.True(value.IsDestinationFileAvailable);
                File.Delete(value.DestinationFilePath);
            },
            onFailure: errors => Assert.Fail($"Compression failed: {errors[0].Message}")
        );
    }

    [Fact]
    public async Task WhenCreatingCompressFile_UsingDirectory_ThenGenerateCompressedFileWithDirectory() {
        // arrange
        var sut = CreateSut();

        await using var sourceFilePath = ResourcesHelper.GetResource(RESOURCE_FILE);
        
        var sourceDirectoryPath = Path.Combine(Path.GetDirectoryName(sourceFilePath.Path) ?? string.Empty, "FolderA");
        var destinationFilePath = Path.Combine(Path.GetTempPath(), "Compress\\LoremIpsum.zip");
        var request = new CompressRequest(destinationFilePath) {
            CompressionLevel = CompressionLevel.Optimal
        }.IncludeDirectory(sourceDirectoryPath);

        // act
        var result = await sut.CompressAsync(request, TestContext.Current.CancellationToken);

        // assert
        result.Match(
            onSuccess: value => {
                Assert.True(value.IsDestinationFileAvailable);
                File.Delete(value.DestinationFilePath);
            },
            onFailure: errors => Assert.Fail($"Compression failed: {errors[0].Message}")
        );
    }
}
