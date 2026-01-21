using System.IO.Compression;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Nameless.Testing.Tools.Attributes;
using Nameless.Testing.Tools.Helpers;

namespace Nameless.Compression;

[UnitTest]
public class ZipArchiveServiceTests {
    private static ZipArchiveService CreateSut(ILogger<ZipArchiveService> logger = null) {
        logger ??= NullLogger<ZipArchiveService>.Instance;

        return new ZipArchiveService(logger);
    }

    [Fact]
    public async Task WhenCreatingCompressFile_WithValidParameters_ThenGenerateCompressedFile() {
        // arrange
        var sut = CreateSut();

        var sourceFilePath = ResourcesHelper.GetFilePath("LoremIpsun.txt");
        var destinationFilePath = ResourcesHelper.GetFilePath("LoremIpsun.zip", ensureFileExistence: false);
        var request = new CompressArchiveRequest {
            DestinationFilePath = destinationFilePath,
            CompressionLevel = CompressionLevel.Optimal
        }.IncludeFile(sourceFilePath, "temp/bar/fuzz");

        // act
        var result = await sut.CompressAsync(request, TestContext.Current.CancellationToken);

        // assert
        result.Match(
            onSuccess: value => {
                Assert.True(value.IsFileAvailable);
                File.Delete(value.FilePath);
            },
            onFailure: errors => Assert.Fail($"Compression failed: {errors[0].Message}")
        );
    }

    [Fact]
    public async Task WhenCreatingCompressFile_UsingDirectory_ThenGenerateCompressedFileWithDirectory() {
        // arrange
        var sut = CreateSut();

        var sourceFilePath = ResourcesHelper.GetFilePath("LoremIpsun.txt");
        var sourceDirectoryPath = Path.Combine(Path.GetDirectoryName(sourceFilePath) ?? string.Empty, "FolderA");
        var destinationFilePath = ResourcesHelper.GetFilePath("Compress\\LoremIpsun.zip", ensureFileExistence: false);
        var request = new CompressArchiveRequest
        {
            DestinationFilePath = destinationFilePath,
            CompressionLevel = CompressionLevel.Optimal
        }.IncludeDirectory(sourceDirectoryPath);

        // act
        var result = await sut.CompressAsync(request, TestContext.Current.CancellationToken);

        // assert
        result.Match(
            onSuccess: value => {
                Assert.True(value.IsFileAvailable);
                File.Delete(value.FilePath);
            },
            onFailure: errors => Assert.Fail($"Compression failed: {errors[0].Message}")
        );
    }
}
