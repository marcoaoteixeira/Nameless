using Microsoft.Extensions.Configuration;
using Moq;

namespace Nameless.Testing.Tools.Mockers;

public sealed class ConfigurationManagerMocker : Mocker<IConfigurationManager> {
    public ConfigurationManagerMocker WithSection(IConfigurationSection returnValue) {
        MockInstance.Setup(mock => mock.GetSection(It.IsAny<string>()))
                    .Returns(returnValue);

        return this;
    }

    public ConfigurationManagerMocker WithSection(string name, IConfigurationSection returnValue) {
        MockInstance.Setup(mock => mock.GetSection(name))
                    .Returns(returnValue);

        return this;
    }
}