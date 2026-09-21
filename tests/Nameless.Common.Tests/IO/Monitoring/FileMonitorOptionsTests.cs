namespace Nameless.IO.Monitoring;

public class FileMonitorOptionsTests
{
    [Fact]
    public void MaxProbeInterval_DefaultsToFiveSeconds()
    {
        Assert.Equal(TimeSpan.FromSeconds(5), new FileMonitorOptions().MaxProbeInterval);
    }

    [Fact]
    public void Excludes_StartWithTheDefaults()
    {
        Assert.Equal(FileMonitorOptions.DefaultExcludes, new FileMonitorOptions().Excludes);
    }

    [Fact]
    public void Excludes_AreNotSharedBetweenInstances()
    {
        var first = new FileMonitorOptions();
        first.Excludes.Clear();

        Assert.Multiple(
            () => Assert.NotEmpty(new FileMonitorOptions().Excludes),
            () => Assert.NotEmpty(FileMonitorOptions.DefaultExcludes)
        );
    }
}
