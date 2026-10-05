using System.IO.Compression;
using Nameless.Compression.Infrastructure;
using Nameless.Testing.Tools.IO;
using static Nameless.Compression.Infrastructure.ZipInspector;

namespace Nameless.Compression;

public sealed class CompressDirectoryTests : IDisposable
{
    private readonly TempDirectory _temp = new();
    private readonly ZipCompressor _sut = ZipCompressorSut.Create();

    public CompressDirectoryTests()
    {
        _temp.CreateFile(SysPath.Combine("docs", "a.txt"), "A");
        _temp.CreateFile(SysPath.Combine("docs", "b.log"), "B");
        _temp.CreateFile(SysPath.Combine("docs", "sub", "c.txt"), "C");
        _temp.CreateFile(SysPath.Combine("docs", "sub", "deep", "d.txt"), "D");
        _temp.CreateDirectory(SysPath.Combine("docs", "empty"));
        _temp.CreateFile("outside.txt", "O");
    }

    public void Dispose() {
        _temp.Dispose();
    }

    private string Docs => _temp.Combine("docs");

    private string Destination => _temp.Combine("out", "docs.zip");

    private static readonly string[] AllFiles = ["docs/a.txt", "docs/b.log", "docs/sub/c.txt", "docs/sub/deep/d.txt"];

    private Task<CompressionResult> CompressDirectoryAsync(
        string directoryPath,
        string? globPattern = null,
        string? destinationFilePath = null,
        CompressionLevel compressionLevel = CompressionLevel.Optimal,
        bool overwrite = false) {
        return _sut.CompressDirectoryAsync(directoryPath, globPattern, destinationFilePath ?? Destination,
            compressionLevel, overwrite, TestContext.Current.CancellationToken);
    }

    // ---- Guard clauses ----------------------------------------------------

    [Fact]
    public async Task Throws_ArgumentNullException_When_DirectoryPath_Is_Null()
    {
        var ex = await Assert.ThrowsAsync<ArgumentNullException>(() => CompressDirectoryAsync(null!));

        Assert.Equal("directoryPath", ex.ParamName);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public async Task Throws_ArgumentException_When_DirectoryPath_Is_Empty_Or_WhiteSpace(string directoryPath)
    {
        var ex = await Assert.ThrowsAsync<ArgumentException>(() => CompressDirectoryAsync(directoryPath));

        Assert.Equal("directoryPath", ex.ParamName);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public async Task Throws_ArgumentException_When_GlobPattern_Is_Empty_Or_WhiteSpace(string globPattern)
    {
        var ex = await Assert.ThrowsAsync<ArgumentException>(() => CompressDirectoryAsync(Docs, globPattern));

        Assert.Equal("globPattern", ex.ParamName);
    }

    [Fact]
    public async Task Throws_ArgumentNullException_When_Destination_Is_Null()
    {
        var ex = await Assert.ThrowsAsync<ArgumentNullException>(
            () => _sut.CompressDirectoryAsync(Docs, null, null!, CompressionLevel.Optimal, false, TestContext.Current.CancellationToken));

        Assert.Equal("destinationFilePath", ex.ParamName);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Throws_ArgumentException_When_Destination_Is_Empty_Or_WhiteSpace(string destination)
    {
        var ex = await Assert.ThrowsAsync<ArgumentException>(() => CompressDirectoryAsync(Docs, destinationFilePath: destination));

        Assert.Equal("destinationFilePath", ex.ParamName);
    }

    [Fact]
    public async Task Throws_ArgumentOutOfRangeException_When_CompressionLevel_Is_Undefined()
    {
        var ex = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => CompressDirectoryAsync(Docs, compressionLevel: (CompressionLevel)(-1)));

        Assert.Equal("compressionLevel", ex.ParamName);
    }

    [Fact]
    public async Task Throws_DirectoryNotFoundException_When_Directory_Does_Not_Exist()
    {
        await Assert.ThrowsAsync<DirectoryNotFoundException>(() => CompressDirectoryAsync(_temp.Combine("missing")));
    }

    [Fact]
    public async Task Throws_DirectoryNotFoundException_When_Path_Is_A_File()
    {
        await Assert.ThrowsAsync<DirectoryNotFoundException>(() => CompressDirectoryAsync(_temp.Combine("outside.txt")));
    }

    // ---- Glob cannot escape the directory --------------------------------

    [Theory]
    [InlineData("..")]
    [InlineData("../*.txt")]
    [InlineData("../**/*")]
    [InlineData("sub/../../*.txt")]
    [InlineData("**/../*")]
    [InlineData(@"..\*.txt")]
    [InlineData(@"sub\..\..\*")]
    [InlineData("/*.txt")]
    [InlineData("/**/*")]
    [InlineData(@"\*.txt")]
    [InlineData(@"\\server\share\*")]
    public async Task Throws_ArgumentException_When_GlobPattern_Could_Escape_The_Directory(string globPattern)
    {
        var ex = await Assert.ThrowsAsync<ArgumentException>(() => CompressDirectoryAsync(Docs, globPattern));

        Assert.Equal("globPattern", ex.ParamName);
        Assert.False(SysFile.Exists(Destination));
    }

    [WindowsTheory]
    [InlineData("C:/*.txt")]
    [InlineData(@"C:\**\*")]
    [InlineData("c:*.txt")]
    public async Task Throws_ArgumentException_When_GlobPattern_Names_A_Drive_On_Windows(string globPattern)
    {
        var ex = await Assert.ThrowsAsync<ArgumentException>(() => CompressDirectoryAsync(Docs, globPattern));

        Assert.Equal("globPattern", ex.ParamName);
    }

    [Theory]
    [InlineData("..*.txt")]
    [InlineData("**/..hidden")]
    [InlineData("sub/a..b/*")]
    public async Task Dots_Inside_A_Segment_Are_Not_Parent_References(string globPattern)
    {
        var result = await CompressDirectoryAsync(Docs, globPattern);

        Assert.True(SysFile.Exists(result.FilePath));
    }

    // ---- Whole directory (null glob) -------------------------------------

    [Fact]
    public async Task Null_Glob_Stores_Whole_Directory_Under_Base_Folder_Name_Including_Empty_Directories()
    {
        var result = await CompressDirectoryAsync(Docs);

        Assert.Equal(Destination, result.FilePath);
        Assert.Equal(
            new Dictionary<string, string?>
            {
                ["docs/a.txt"] = "A",
                ["docs/b.log"] = "B",
                ["docs/sub/c.txt"] = "C",
                ["docs/sub/deep/d.txt"] = "D",
                ["docs/empty/"] = null,
            },
            await ReadEntriesAsync(result.FilePath));
    }

    [Fact]
    public async Task Directory_Path_With_Trailing_Separator_Is_Supported()
    {
        var result = await CompressDirectoryAsync(Docs + SysPath.DirectorySeparatorChar);

        Assert.Contains("docs/a.txt", (await ReadEntriesAsync(result.FilePath)).Keys);
    }

    [Fact]
    public async Task Empty_Directory_Produces_Archive_With_Only_Base_Folder_Entry()
    {
        var directory = _temp.CreateDirectory("nothing");

        var result = await CompressDirectoryAsync(directory);

        Assert.Equal(new Dictionary<string, string?> { ["nothing/"] = null }, await ReadEntriesAsync(result.FilePath));
    }

    [Fact]
    public async Task Hidden_Files_Are_Included()
    {
        var hidden = _temp.CreateFile(SysPath.Combine("docs", ".hidden"), "h");
        SysFile.SetAttributes(hidden, SysFile.GetAttributes(hidden) | FileAttributes.Hidden);

        var result = await CompressDirectoryAsync(Docs);

        Assert.Contains("docs/.hidden", (await ReadEntriesAsync(result.FilePath)).Keys);
    }

    [Fact]
    public async Task Relative_Paths_Are_Resolved_And_Full_Destination_Is_Returned()
    {
        var relativeDirectory = SysPath.GetRelativePath(Environment.CurrentDirectory, Docs);
        var relativeDestination = SysPath.GetRelativePath(Environment.CurrentDirectory, Destination);

        var result = await CompressDirectoryAsync(relativeDirectory, destinationFilePath: relativeDestination);

        Assert.Equal(Destination, result.FilePath);
        Assert.Contains("docs/a.txt", (await ReadEntriesAsync(result.FilePath)).Keys);
    }

    // ---- Glob matching ----------------------------------------------------

    [Fact]
    public async Task Match_All_Glob_Stores_Files_Only()
    {
        var result = await CompressDirectoryAsync(Docs, "**/*");

        Assert.Equal(AllFiles, (await ReadEntriesAsync(result.FilePath)).Keys.Order());
    }

    [Fact]
    public async Task Single_Star_Matches_Top_Level_Only()
    {
        var result = await CompressDirectoryAsync(Docs, "*.txt");

        Assert.Equal(new Dictionary<string, string?> { ["docs/a.txt"] = "A" }, await ReadEntriesAsync(result.FilePath));
    }

    [Fact]
    public async Task Double_Star_Matches_Recursively_And_Keeps_Relative_Structure()
    {
        var result = await CompressDirectoryAsync(Docs, "**/*.txt");

        Assert.Equal(
            new Dictionary<string, string?>
            {
                ["docs/a.txt"] = "A",
                ["docs/sub/c.txt"] = "C",
                ["docs/sub/deep/d.txt"] = "D",
            },
            await ReadEntriesAsync(result.FilePath));
    }

    [Fact]
    public async Task Pattern_With_Path_Segments_Is_Supported()
    {
        var result = await CompressDirectoryAsync(Docs, "sub/*/*.txt");

        Assert.Equal(new Dictionary<string, string?> { ["docs/sub/deep/d.txt"] = "D" }, await ReadEntriesAsync(result.FilePath));
    }

    [Fact]
    public async Task Hidden_Files_Are_Matched()
    {
        var hidden = _temp.CreateFile(SysPath.Combine("docs", "h.txt"), "H");
        SysFile.SetAttributes(hidden, SysFile.GetAttributes(hidden) | FileAttributes.Hidden);

        var result = await CompressDirectoryAsync(Docs, "*.txt");

        Assert.Contains("docs/h.txt", (await ReadEntriesAsync(result.FilePath)).Keys);
    }

    [Fact]
    public async Task No_Matches_Produces_Empty_Archive()
    {
        var result = await CompressDirectoryAsync(Docs, "*.nope");

        Assert.True(SysFile.Exists(result.FilePath));
        Assert.Empty(await ReadEntriesAsync(result.FilePath));
    }

    // ---- Destination & overwrite -----------------------------------------

    [Theory]
    [InlineData(null)]
    [InlineData("**/*")]
    public async Task Destination_Inside_Directory_Is_Not_Archived_Into_Itself(string? globPattern)
    {
        var destination = _temp.Combine("docs", "self.zip");
        SysFile.WriteAllText(destination, "stale archive");

        var result = await CompressDirectoryAsync(Docs, globPattern, destination, overwrite: true);

        Assert.DoesNotContain("docs/self.zip", (await ReadEntriesAsync(result.FilePath)).Keys);
        Assert.Equal(AllFiles, (await ReadEntriesAsync(result.FilePath)).Keys.Where(name => !name.EndsWith('/')).Order());
    }

    [Fact]
    public async Task Throws_IOException_And_Keeps_Existing_Archive_When_Overwrite_Is_False()
    {
        var destination = _temp.CreateFile("docs.zip", "existing");

        await Assert.ThrowsAsync<IOException>(() => CompressDirectoryAsync(Docs, destinationFilePath: destination, overwrite: false));

        Assert.Equal("existing", await SysFile.ReadAllTextAsync(destination, TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task Replaces_Existing_Archive_When_Overwrite_Is_True()
    {
        var destination = _temp.CreateFile("docs.zip", "existing");

        var result = await CompressDirectoryAsync(Docs, "*.txt", destination, overwrite: true);

        Assert.Equal("A", (await ReadEntriesAsync(result.FilePath))["docs/a.txt"]);
    }

    // ---- Cancellation -----------------------------------------------------

    [Theory]
    [InlineData(null)]
    [InlineData("**/*")]
    public async Task Cancelled_Token_Throws_And_Leaves_No_Archive(string? globPattern)
    {
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => _sut.CompressDirectoryAsync(Docs, globPattern, Destination, CompressionLevel.Optimal, false, cts.Token));

        Assert.False(SysFile.Exists(Destination));
    }
}
