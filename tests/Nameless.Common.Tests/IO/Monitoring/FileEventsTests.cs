namespace Nameless.IO.Monitoring;

[UnitTest]
public class FileEventsTests {
    private static readonly string Dir = SysPath.Combine(SysPath.GetTempPath(), "events");

    [Fact]
    public void FileCreatedEvent_ExposesPathParts() {
        // arrange
        var path = SysPath.Combine(Dir, "a.txt");

        // act
        var sut = new FileCreatedEvent(path);

        // assert
        Assert.Multiple(
            () => Assert.Equal(path, sut.CurrentPath),
            () => Assert.Equal(Dir, sut.Directory),
            () => Assert.Equal("a.txt", sut.Name)
        );
    }

    [Fact]
    public void FileChangedEvent_ExposesPathParts() {
        // act
        var sut = new FileChangedEvent(SysPath.Combine(Dir, "b.txt"));

        // assert
        Assert.Equal("b.txt", sut.Name);
    }

    [Fact]
    public void FileDeletedEvent_ExposesPathParts() {
        // act
        var sut = new FileDeletedEvent(SysPath.Combine(Dir, "c.txt"));

        // assert
        Assert.Equal("c.txt", sut.Name);
    }

    [Fact]
    public void FileRenamedEvent_ExposesPreviousAndCurrentPaths() {
        // arrange
        var previous = SysPath.Combine(Dir, "old.txt");
        var current = SysPath.Combine(Dir, "new.txt");

        // act
        var sut = new FileRenamedEvent(previous, current);

        // assert
        Assert.Multiple(
            () => Assert.Equal(previous, sut.PreviousPath),
            () => Assert.Equal("old.txt", sut.PreviousName),
            () => Assert.Equal(current, sut.CurrentPath),
            () => Assert.Equal("new.txt", sut.Name)
        );
    }

    [Fact]
    public void FileEvent_WithPathWithoutDirectory_ReturnsEmptyDirectory() {
        // act
        var sut = new FileCreatedEvent(SysPath.GetPathRoot(Dir)!);

        // assert
        Assert.Equal(string.Empty, sut.Directory);
    }

    [Fact]
    public void FileMonitorErrorEvent_ExposesException() {
        // arrange
        var exception = new InvalidOperationException("boom");

        // act
        var sut = new FileMonitorErrorEvent(exception);

        // assert
        Assert.Same(exception, sut.Exception);
    }

    [Fact]
    public void FileLockedTooLongException_ExposesPathAndDuration() {
        // act
        var sut = new FileLockedTooLongException("a.txt", TimeSpan.FromMinutes(6));

        // assert
        Assert.Multiple(
            () => Assert.Equal("a.txt", sut.FilePath),
            () => Assert.Equal(TimeSpan.FromMinutes(6), sut.LockedFor),
            () => Assert.Contains("a.txt", sut.Message)
        );
    }

    [Fact]
    public void FileMonitorOptions_HasExpectedDefaults() {
        // act
        var sut = new FileMonitorOptions();

        // assert
        Assert.Multiple(
            () => Assert.Equal(TimeSpan.FromMilliseconds(500), sut.QuietPeriod),
            () => Assert.Equal(TimeSpan.FromMilliseconds(500), sut.ProbeInterval),
            () => Assert.Equal(TimeSpan.FromSeconds(5), sut.MaxProbeInterval),
            () => Assert.Equal(TimeSpan.FromMinutes(5), sut.LockedTooLongAfter),
            () => Assert.Equal(TimeSpan.FromMilliseconds(250), sut.ReplaceGracePeriod),
            () => Assert.Equal(FileMonitorOptions.DefaultExcludes, sut.Excludes),
            () => Assert.Equal(64 * 1024, sut.InternalBufferSize)
        );
    }
}
