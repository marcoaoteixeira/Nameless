using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Nameless.Localization.Json.Options;
using Nameless.Mockers;

namespace Nameless.Localization.Json.Infrastructure;

public class ResourceManagerTests {
    private const string PT_BR = "pt-BR";
    private static ILogger<ResourceManager> ResourceManagerLogger => NullLogger<ResourceManager>.Instance;

    [Test]
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

        var options = Microsoft.Extensions.Options.Options.Create(new LocalizationOptions());
        var sut = new ResourceManager(fileProvider, options, ResourceManagerLogger);

        // act
        var resource = sut.GetResource(typeof(ResourceManagerTests).Namespace ?? string.Empty,
                                       nameof(ResourceManagerTests),
                                       PT_BR);

        // assert
        Assert.Multiple(() => {
            Assert.That(resource, Is.Not.Null);
            Assert.That(resource.Culture, Is.Not.Null);
            Assert.That(resource.Path, Is.Not.Null);
            Assert.That(resource.IsAvailable, Is.True);
        });
        Assert.That(resource, Is.Not.Null);
    }
}