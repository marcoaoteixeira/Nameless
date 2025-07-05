using Microsoft.Extensions.Configuration;

namespace Nameless.Testing.Tools.Mockers;

public sealed class ConfigurationSectionMocker : Mocker<IConfigurationSection> {
    public ConfigurationSectionMocker WithKey(string returnValue) {
        MockInstance.Setup(mock => mock.Key)
                    .Returns(returnValue);

        return this;
    }

    public ConfigurationSectionMocker WithValue(string returnValue) {
        MockInstance.Setup(mock => mock.Value)
                    .Returns(returnValue);

        return this;
    }
}