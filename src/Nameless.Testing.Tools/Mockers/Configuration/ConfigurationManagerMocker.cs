using Microsoft.Extensions.Configuration;
using Moq;

namespace Nameless.Testing.Tools.Mockers.Configuration;

public class ConfigurationManagerMocker : Mocker<IConfigurationManager> {
    public ConfigurationManagerMocker WithGetSection(IConfigurationSection returnValue) {
        MockInstance.Setup(mock => mock.GetSection(It.IsAny<string>()))
                    .Returns(returnValue);

        return this;
    }

    public ConfigurationManagerMocker WithGetSection(string name, IConfigurationSection returnValue) {
        MockInstance.Setup(mock => mock.GetSection(name))
                    .Returns(returnValue);

        return this;
    }
}