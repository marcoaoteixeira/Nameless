using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Moq;

namespace Nameless.Testing.Tools.Mockers.Configuration;

public class ConfigurationMocker : Mocker<IConfiguration> {
    public ConfigurationMocker WithIndexer(string returnValue) {
        MockInstance.SetupGet(mock => mock[It.IsAny<string>()])
                    .Returns(returnValue);

        return this;
    }

    public ConfigurationMocker WithIndexer(string key, string returnValue) {
        MockInstance.SetupGet(mock => mock[key])
                    .Returns(returnValue);

        return this;
    }

    public ConfigurationMocker WithGetSection(IConfigurationSection returnValue) {
        MockInstance.Setup(mock => mock.GetSection(It.IsAny<string>()))
                    .Returns(returnValue);

        return this;
    }

    public ConfigurationMocker WithGetSection(string key, IConfigurationSection returnValue) {
        MockInstance.Setup(mock => mock.GetSection(key))
                    .Returns(returnValue);

        return this;
    }

    public ConfigurationMocker WithGetChildren(params IEnumerable<IConfigurationSection> returnValue) {
        MockInstance.Setup(mock => mock.GetChildren())
                    .Returns(returnValue);

        return this;
    }

    public ConfigurationMocker WithGetReloadToken(IChangeToken returnValue) {
        MockInstance.Setup(mock => mock.GetReloadToken())
                    .Returns(returnValue);

        return this;
    }
}