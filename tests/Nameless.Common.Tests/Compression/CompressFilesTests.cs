using System.IO.Compression;
using Nameless.Compression.Infrastructure;
using Nameless.Compression.Internals;
using Nameless.Testing.Tools.IO;
using static Nameless.Compression.Infrastructure.ZipInspector;

namespace Nameless.Compression;

public sealed class CompressFilesTests : IDisposable
{
    private readonly TempDirectory _temp = new();
    private readonly ZipCompressor _sut = ZipCompressorSut.Create();

    public void Dispose() {
        _temp.Dispose();
    }

    private string Destination => _temp.Combine("out", "bundle.zip");

    private Task<CompressionResult> CompressFilesAsync(
        IEnumerable<string> filePaths,
        string? destinationFilePath = null,
        CompressionLevel compressionLevel = CompressionLevel.Optimal,
        bool overwrite = false) {
        return _sut.CompressFilesAsync(filePaths, destinationFilePath ?? Destination, compressionLevel, overwrite,
            TestContext.Current.CancellationToken);
    }

    // ---- Guard clauses ----------------------------------------------------

    [Fact]
    public async Task Throws_ArgumentNullException_When_List_Is_Null()
    {
        var ex = await Assert.ThrowsAsync<ArgumentNullException>(() => CompressFilesAsync(null!));

        Assert.Equal("filePaths", ex.ParamName);
    }

    [Fact]
    public async Task Throws_ArgumentException_When_List_Is_Empty()
    {
        var ex = await Assert.ThrowsAsync<ArgumentException>(() => CompressFilesAsync([]));

        Assert.Equal("filePaths", ex.ParamName);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Throws_ArgumentException_When_List_Contains_Null_Empty_Or_WhiteSpace(string? invalid)
    {
        var file = _temp.CreateFile("a.txt", "a");

        var ex = await Assert.ThrowsAsync<ArgumentException>(() => CompressFilesAsync([file, invalid!]));

        Assert.Equal("filePaths", ex.ParamName);
    }

    [Fact]
    public async Task Throws_ArgumentNullException_When_Destination_Is_Null()
    {
        var file = _temp.CreateFile("a.txt", "a");

        var ex = await Assert.ThrowsAsync<ArgumentNullException>(
            () => _sut.CompressFilesAsync([file], null!, CompressionLevel.Optimal, false, TestContext.Current.CancellationToken));

        Assert.Equal("destinationFilePath", ex.ParamName);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Throws_ArgumentException_When_Destination_Is_Empty_Or_WhiteSpace(string destination)
    {
        var file = _temp.CreateFile("a.txt", "a");

        var ex = await Assert.ThrowsAsync<ArgumentException>(() => CompressFilesAsync([file], destination));

        Assert.Equal("destinationFilePath", ex.ParamName);
    }

    [Fact]
    public async Task Throws_ArgumentOutOfRangeException_When_CompressionLevel_Is_Undefined()
    {
        var file = _temp.CreateFile("a.txt", "a");

        var ex = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => CompressFilesAsync([file], compressionLevel: (CompressionLevel)42));

        Assert.Equal("compressionLevel", ex.ParamName);
    }

    [Fact]
    public async Task Throws_FileNotFoundException_When_A_File_Does_Not_Exist()
    {
        var file = _temp.CreateFile("a.txt", "a");
        var missing = _temp.Combine("missing.txt");

        var ex = await Assert.ThrowsAsync<FileNotFoundException>(() => CompressFilesAsync([file, missing]));

        Assert.Equal(missing, ex.FileName);
    }

    [Fact]
    public async Task Throws_FileNotFoundException_When_A_Path_Is_A_Directory()
    {
        var file = _temp.CreateFile("a.txt", "a");
        var directory = _temp.CreateDirectory("dir");

        await Assert.ThrowsAsync<FileNotFoundException>(() => CompressFilesAsync([file, directory]));
    }

    // ---- Single file ------------------------------------------------------

    [Fact]
    public async Task Single_File_Keeps_Its_Full_Path_Without_Root()
    {
        var file = _temp.CreateFile("report.txt", "hello world");

        var result = await CompressFilesAsync([file]);

        Assert.Equal(Destination, result.FilePath);
        Assert.Equal(new Dictionary<string, string?> { [EntryFor(file)] = "hello world" }, await ReadEntriesAsync(result.FilePath));
    }

    [Fact]
    public async Task Empty_File_Produces_Archive_With_Empty_Entry()
    {
        var file = _temp.CreateFile("empty.txt");

        var result = await CompressFilesAsync([file]);

        Assert.Equal(new Dictionary<string, string?> { [EntryFor(file)] = "" }, await ReadEntriesAsync(result.FilePath));
    }

    [Fact]
    public async Task Relative_Paths_Are_Resolved_And_Full_Destination_Is_Returned()
    {
        var file = _temp.CreateFile("rel.txt", "r");
        var relativeFile = SysPath.GetRelativePath(Environment.CurrentDirectory, file);
        var relativeDestination = SysPath.GetRelativePath(Environment.CurrentDirectory, Destination);

        var result = await CompressFilesAsync([relativeFile], relativeDestination);

        Assert.Equal(Destination, result.FilePath);
        Assert.Equal([EntryFor(file)], (await ReadEntriesAsync(result.FilePath)).Keys);
    }

    [Theory]
    [InlineData(CompressionLevel.Optimal)]
    [InlineData(CompressionLevel.Fastest)]
    [InlineData(CompressionLevel.NoCompression)]
    [InlineData(CompressionLevel.SmallestSize)]
    public async Task Every_CompressionLevel_Produces_A_Valid_Archive(CompressionLevel level)
    {
        var content = string.Concat(Enumerable.Repeat("compress me ", 500));
        var file = _temp.CreateFile("data.txt", content);

        var result = await CompressFilesAsync([file], compressionLevel: level);

        Assert.Equal(content, (await ReadEntriesAsync(result.FilePath))[EntryFor(file)]);
    }

    [Fact]
    public async Task NoCompression_Stores_Entry_Uncompressed()
    {
        var file = _temp.CreateFile("data.txt", new string('a', 10_000));

        var result = await CompressFilesAsync([file], compressionLevel: CompressionLevel.NoCompression);

        var entry = Assert.Single(await ReadEntryMetadataAsync(result.FilePath));
        Assert.Equal(entry.Length, entry.CompressedLength);
    }

    // ---- Many files -------------------------------------------------------

    [Fact]
    public async Task Entries_Are_Full_Paths_Without_Root()
    {
        var a = _temp.CreateFile(SysPath.Combine("proj", "x", "same.txt"), "1");
        var b = _temp.CreateFile(SysPath.Combine("proj", "y", "deep", "same.txt"), "2");

        var result = await CompressFilesAsync([a, b]);

        var entries = await ReadEntriesAsync(result.FilePath);
        Assert.Equal(new Dictionary<string, string?> { [EntryFor(a)] = "1", [EntryFor(b)] = "2" }, entries);
        Assert.All(entries.Keys, name =>
        {
            Assert.DoesNotContain(':', name);
            Assert.False(name.StartsWith('/'));
        });
    }

    [Fact]
    public async Task Duplicate_Paths_Are_Included_Once()
    {
        var a = _temp.CreateFile(SysPath.Combine("proj", "a.txt"), "A");
        var relative = SysPath.GetRelativePath(Environment.CurrentDirectory, a);

        var result = await CompressFilesAsync([a, a, relative]);

        Assert.Equal([EntryFor(a)], (await ReadEntriesAsync(result.FilePath)).Keys);
    }

    [WindowsFact]
    public async Task Duplicate_Paths_Differing_Only_By_Case_Are_Included_Once_On_Windows()
    {
        var a = _temp.CreateFile(SysPath.Combine("proj", "a.txt"), "A");

        var result = await CompressFilesAsync([a, a.ToUpperInvariant()]);

        Assert.Single(await ReadEntriesAsync(result.FilePath));
    }

    [Fact]
    public async Task List_Is_Enumerated_Only_Once()
    {
        var a = _temp.CreateFile(SysPath.Combine("proj", "a.txt"), "A");
        var enumerations = 0;

        IEnumerable<string> Source()
        {
            enumerations++;
            yield return a;
        }

        await CompressFilesAsync(Source());

        Assert.Equal(1, enumerations);
    }

    [Fact]
    public async Task Round_Trip_Recreates_Rootless_Paths_Under_Destination()
    {
        var a = _temp.CreateFile(SysPath.Combine("proj", "a.txt"), "A");
        var archive = await CompressFilesAsync([a]);

        var restored = await _sut.DecompressAsync(archive.FilePath, _temp.Combine("restored"), null, false, TestContext.Current.CancellationToken);

        Assert.Equal("A", await SysFile.ReadAllTextAsync(ExtractedPath(restored, a), TestContext.Current.CancellationToken));
    }

    // ---- Destination & overwrite -----------------------------------------

    [Fact]
    public async Task Missing_Parent_Directories_Of_Destination_Are_Created()
    {
        var file = _temp.CreateFile("a.txt", "a");
        var destination = _temp.Combine("out", "nested", "custom.zip");

        var result = await CompressFilesAsync([file], destination);

        Assert.Equal(destination, result.FilePath);
        Assert.True(SysFile.Exists(destination));
    }

    [Fact]
    public async Task Throws_IOException_And_Keeps_Existing_Archive_When_Overwrite_Is_False()
    {
        var file = _temp.CreateFile("a.txt", "a");
        var destination = _temp.CreateFile("a.zip", "existing");

        await Assert.ThrowsAsync<IOException>(() => CompressFilesAsync([file], destination, overwrite: false));

        Assert.Equal("existing", await SysFile.ReadAllTextAsync(destination, TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task Replaces_Existing_Archive_When_Overwrite_Is_True()
    {
        var file = _temp.CreateFile("a.txt", "fresh");
        var destination = _temp.CreateFile("a.zip", "existing");

        var result = await CompressFilesAsync([file], destination, overwrite: true);

        Assert.Equal("fresh", (await ReadEntriesAsync(result.FilePath))[EntryFor(file)]);
    }

    [Fact]
    public async Task No_Temporary_Files_Are_Left_Behind()
    {
        var file = _temp.CreateFile("a.txt", "a");

        await CompressFilesAsync([file], _temp.Combine("a.zip"));

        Assert.Equal(["a.txt", "a.zip"], SysDirectory.GetFiles(_temp.Path).Select(SysPath.GetFileName).Order());
    }

    // ---- Cancellation -----------------------------------------------------

    [Fact]
    public async Task Cancelled_Token_Throws_And_Leaves_No_Archive()
    {
        var a = _temp.CreateFile(SysPath.Combine("proj", "a.txt"), "A");
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => _sut.CompressFilesAsync([a], Destination, CompressionLevel.Optimal, false, cts.Token));

        Assert.False(SysFile.Exists(Destination));
    }
}

/// <summary>
/// Pure path math for <see cref="ZipCompressor.CompressFilesAsync"/>; no file system involved, so other drives and
/// shares can be simulated.
/// </summary>
public sealed class FileListLayoutTests
{
    private static string[] EntryNames(IReadOnlyList<FileListLayout.Entry> entries) {
        return [.. entries.Select(e => e.EntryName)];
    }

    [WindowsFact]
    public void Entry_Is_Full_Path_Without_Drive()
    {
        var entries = FileListLayout.Create(
            [@"C:\work\proj\a\1.txt", @"C:\work\proj\b\2.txt"],
            StringComparison.OrdinalIgnoreCase);

        Assert.Equal(["work/proj/a/1.txt", "work/proj/b/2.txt"], EntryNames(entries));
    }

    [WindowsFact]
    public void Files_On_Different_Drives_Have_No_Drive_Information()
    {
        var entries = FileListLayout.Create(
            [@"C:\a\x.txt", @"D:\b\y.txt"],
            StringComparison.OrdinalIgnoreCase);

        Assert.Equal(["a/x.txt", "b/y.txt"], EntryNames(entries));
    }

    [WindowsFact]
    public void Network_Paths_Have_No_Server_Or_Share_Information()
    {
        var entries = FileListLayout.Create(
            [@"\\server\share\data\b.txt", @"C:\c.txt"],
            StringComparison.OrdinalIgnoreCase);

        Assert.Equal(["data/b.txt", "c.txt"], EntryNames(entries));
    }

    [WindowsFact]
    public void Same_Relative_Path_On_Different_Drives_Throws()
    {
        var ex = Assert.Throws<ArgumentException>(() => FileListLayout.Create(
            [@"C:\a\x.txt", @"D:\a\x.txt"],
            StringComparison.OrdinalIgnoreCase));

        Assert.Equal("filePaths", ex.ParamName);
        Assert.Contains("a/x.txt", ex.Message);
    }

    [WindowsFact]
    public void Same_Relative_Path_On_Drive_And_Share_Throws()
    {
        Assert.Throws<ArgumentException>(() => FileListLayout.Create(
            [@"C:\a\x.txt", @"\\server\share\a\x.txt"],
            StringComparison.OrdinalIgnoreCase));
    }

    [WindowsFact]
    public void Clash_Detection_Honors_Case_Insensitivity()
    {
        Assert.Throws<ArgumentException>(() => FileListLayout.Create(
            [@"C:\A\X.txt", @"D:\a\x.txt"],
            StringComparison.OrdinalIgnoreCase));
    }

    [WindowsFact]
    public void File_Clashing_With_A_Directory_Of_Another_Entry_Throws()
    {
        // 'a' would be both a file (from C:) and a directory (from D:) inside the archive.
        Assert.Throws<ArgumentException>(() => FileListLayout.Create(
            [@"C:\a", @"D:\a\x.txt"],
            StringComparison.OrdinalIgnoreCase));
    }

    [WindowsFact]
    public void Source_Paths_Are_Preserved_In_Order()
    {
        string[] paths = [@"C:\p\b.txt", @"D:\p\a.txt"];

        var entries = FileListLayout.Create(paths, StringComparison.OrdinalIgnoreCase);

        Assert.Equal(paths, entries.Select(e => e.SourcePath));
    }
}
