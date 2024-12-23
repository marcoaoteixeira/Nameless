using Moq;
using Nameless.Localization.Json.Infrastructure;
using Nameless.Localization.Json.Objects;
using Nameless.Mockers;

namespace Nameless.Localization.Json.Mockers;
public class ResourceManagerMocker : MockerBase<IResourceManager> {
    public ResourceManagerMocker WithResource(Resource resource) {
        Mock
            .Setup(mock => mock.GetResource(It.IsAny<string>(), It.IsAny<string>(), resource.Culture))
            .Returns(resource);

        return this;
    }
}
