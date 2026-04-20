using System.IO.Compression;
using Nameless.Compression;
using Nameless.Compression.Requests;
using Nameless.Compression.Responses;
using Nameless.ObjectModel;

namespace Nameless;

public class CompressTests {
    // --- FileEntry ---

    [Fact]
    public void FileEntry_Constructor_SetsPathAndDirectoryPath() {
        // act
        var entry = new FileEntry("/tmp/file.txt", "/archive/docs");

        // assert
        Assert.Multiple(() => {
            Assert.Equal("/tmp/file.txt", entry.Path);
            Assert.Equal("/archive/docs", entry.DirectoryPath);
        });
    }

    [Fact]
    public void FileEntry_WithNullDirectoryPath_IsNull() {
        // act
        var entry = new FileEntry("/tmp/file.txt", null);

        // assert
        Assert.Null(entry.DirectoryPath);
    }

    // --- CompressMetadata ---

    [Fact]
    public void CompressMetadata_IsFileAvailable_WhenFileDoesNotExist_ReturnsFalse() {
        // act
        var meta = new CompressMetadata("/nonexistent/path.zip");

        // assert
        Assert.False(meta.IsFileAvailable);
    }

    [Fact]
    public void CompressMetadata_IsFileAvailable_WhenFileExists_ReturnsTrue() {
        // arrange
        var path = Path.GetTempFileName();
        try {
            // act
            var meta = new CompressMetadata(path);

            // assert
            Assert.True(meta.IsFileAvailable);
        } finally {
            File.Delete(path);
        }
    }

    // --- DecompressMetadata ---

    [Fact]
    public void DecompressMetadata_IsDirectoryAvailable_WhenDirectoryDoesNotExist_ReturnsFalse() {
        // act
        var meta = new DecompressMetadata("/nonexistent/directory");

        // assert
        Assert.False(meta.IsDirectoryAvailable);
    }

    [Fact]
    public void DecompressMetadata_IsDirectoryAvailable_WhenDirectoryExists_ReturnsTrue() {
        // arrange
        var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(path);
        try {
            // act
            var meta = new DecompressMetadata(path);

            // assert
            Assert.True(meta.IsDirectoryAvailable);
        } finally {
            Directory.Delete(path);
        }
    }

    // --- CompressRequest ---

    [Fact]
    public void CompressRequest_Constructor_WithValidPath_SetsDestinationFilePath() {
        // act
        var request = new CompressRequest("/output/archive.zip");

        // assert
        Assert.Equal("/output/archive.zip", request.DestinationFilePath);
    }

    [Fact]
    public void CompressRequest_Constructor_WithNullPath_ThrowsArgumentNullException() {
        // act & assert
        Assert.Throws<ArgumentNullException>(() => new CompressRequest(null!));
    }

    [Fact]
    public void CompressRequest_Constructor_WithEmptyPath_ThrowsArgumentException() {
        // act & assert
        Assert.Throws<ArgumentException>(() => new CompressRequest(string.Empty));
    }

    [Fact]
    public void CompressRequest_DefaultCompressionLevel_IsOptimal() {
        // act
        var request = new CompressRequest("/output/archive.zip");

        // assert
        Assert.Equal(CompressionLevel.Optimal, request.CompressionLevel);
    }

    [Fact]
    public void CompressRequest_IncludeFile_WhenFileExists_AddsToFiles() {
        // arrange
        var tempFile = Path.GetTempFileName();
        try {
            var request = new CompressRequest("/output/archive.zip");

            // act
            request.IncludeFile(tempFile);

            // assert
            Assert.Single(request.Files);
        } finally {
            File.Delete(tempFile);
        }
    }

    [Fact]
    public void CompressRequest_IncludeFile_WhenFileNotFound_ThrowsFileNotFoundException() {
        // arrange
        var request = new CompressRequest("/output/archive.zip");

        // act & assert
        Assert.Throws<FileNotFoundException>(() => request.IncludeFile("/nonexistent/file.txt"));
    }

    [Fact]
    public void CompressRequest_IncludeFile_ReturnsSameInstance() {
        // arrange
        var tempFile = Path.GetTempFileName();
        try {
            var request = new CompressRequest("/output/archive.zip");

            // act
            var returned = request.IncludeFile(tempFile);

            // assert
            Assert.Same(request, returned);
        } finally {
            File.Delete(tempFile);
        }
    }

    [Fact]
    public void CompressRequest_IncludeDirectory_WhenDirectoryExists_AddsFiles() {
        // arrange
        var dir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(dir);
        var file = Path.Combine(dir, "test.txt");
        File.WriteAllText(file, "content");
        try {
            var request = new CompressRequest("/output/archive.zip");

            // act
            request.IncludeDirectory(dir);

            // assert
            Assert.NotEmpty(request.Files);
        } finally {
            File.Delete(file);
            Directory.Delete(dir);
        }
    }

    [Fact]
    public void CompressRequest_IncludeDirectory_WhenDirectoryNotFound_ThrowsDirectoryNotFoundException() {
        // arrange
        var request = new CompressRequest("/output/archive.zip");

        // act & assert
        Assert.Throws<DirectoryNotFoundException>(
            () => request.IncludeDirectory("/nonexistent/directory")
        );
    }

    // --- DecompressRequest ---

    [Fact]
    public void DecompressRequest_Constructor_WithValidPath_SetsSourceFilePath() {
        // act
        var request = new DecompressRequest("/input/archive.zip");

        // assert
        Assert.Equal("/input/archive.zip", request.SourceFilePath);
    }

    [Fact]
    public void DecompressRequest_Constructor_WithNullPath_ThrowsArgumentNullException() {
        // act & assert
        Assert.Throws<ArgumentNullException>(() => new DecompressRequest(null!));
    }

    [Fact]
    public void DecompressRequest_DefaultDestinationDirectoryPath_IsNull() {
        // act
        var request = new DecompressRequest("/input/archive.zip");

        // assert
        Assert.Null(request.DestinationDirectoryPath);
    }

    // --- CompressResponse ---

    [Fact]
    public void CompressResponse_FromMetadata_IsSuccess() {
        // arrange
        var meta = new CompressMetadata("/output/archive.zip");

        // act
        CompressResponse response = meta;

        // assert
        Assert.True(response.Success);
    }

    [Fact]
    public void CompressResponse_FromError_IsFailure() {
        // act
        CompressResponse response = Error.Failure("compress failed");

        // assert
        Assert.Multiple(() => {
            Assert.False(response.Success);
            Assert.Single(response.Errors);
        });
    }

    [Fact]
    public void CompressResponse_FromErrorArray_IsFailure() {
        // act
        CompressResponse response = new[] { Error.Failure("err1"), Error.Failure("err2") };

        // assert
        Assert.Multiple(() => {
            Assert.False(response.Success);
            Assert.Equal(2, response.Errors.Length);
        });
    }

    // --- DecompressResponse ---

    [Fact]
    public void DecompressResponse_FromMetadata_IsSuccess() {
        // arrange
        var meta = new DecompressMetadata("/output/dir");

        // act
        DecompressResponse response = meta;

        // assert
        Assert.True(response.Success);
    }

    [Fact]
    public void DecompressResponse_FromError_IsFailure() {
        // act
        DecompressResponse response = Error.Failure("decompress failed");

        // assert
        Assert.Multiple(() => {
            Assert.False(response.Success);
            Assert.Single(response.Errors);
        });
    }

    [Fact]
    public void DecompressResponse_FromErrorArray_IsFailure() {
        // act
        DecompressResponse response = new[] { Error.Failure("err") };

        // assert
        Assert.False(response.Success);
    }
}
