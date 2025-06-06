using Microsoft.Extensions.Configuration;

namespace Nameless.Testing.Tools.Mockers;

public sealed class ConfigurationManagerMocker : MockerBase<IConfigurationManager> {
    public ConfigurationManagerMocker WithSection(string name, IConfigurationSection section) {
        MockInstance.Setup(mock => mock.GetSection(name))
                    .Returns(section);

        return this;
    }
}