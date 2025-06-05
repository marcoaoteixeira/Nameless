using Microsoft.Extensions.Configuration;

namespace Nameless.Testing.Tools.Mockers;

public sealed class ConfigurationSectionMocker : MockerBase<IConfigurationSection> {
    public ConfigurationSectionMocker WithKey(string key) {
        MockInstance.Setup(mock => mock.Key)
                    .Returns(key);

        return this;
    }

    public ConfigurationSectionMocker WithValue(string value) {
        MockInstance.Setup(mock => mock.Value)
                    .Returns(value);

        return this;
    }
}