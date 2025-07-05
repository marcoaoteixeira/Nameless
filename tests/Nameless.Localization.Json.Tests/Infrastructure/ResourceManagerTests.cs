using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Nameless.Null;
using Nameless.Testing.Tools.Mockers;

namespace Nameless.Localization.Json.Infrastructure;

public class ResourceManagerTests {
    private const string PT_BR = "pt-BR";
    private static ILogger<ResourceManager> ResourceManagerLogger => NullLogger<ResourceManager>.Instance;

    [Fact]
    public void WhenGettingResource_ShouldReturnResourceForGivenParameters() {
        // arrange
        var fileInfo = new FileInfoMocker().WithExists(true)
                                           .WithCreateReadStream("[]")
                                           .Build();
        var changeToken = new ChangeTokenMocker().WithRegisterChangeCallback(NullDisposable.Instance)
                                                 .Build();
        var fileProvider = new FileProviderMocker().WithFileInfo(fileInfo)
                                                   .WithWatch(changeToken)
                                                   .Build();

        var options = Microsoft.Extensions.Options.Options.Create(new JsonLocalizationOptions());
        var sut = new ResourceManager(fileProvider, options, ResourceManagerLogger);

        // act
        var resource = sut.GetResource(typeof(ResourceManagerTests).Namespace ?? string.Empty,
            nameof(ResourceManagerTests),
            PT_BR);

        // assert
        Assert.Multiple(() => {
            Assert.NotNull(resource);
            Assert.NotNull(resource.Culture);
            Assert.NotNull(resource.Path);
            Assert.True(resource.IsAvailable);
        });
    }
}