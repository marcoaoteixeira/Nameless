﻿using Microsoft.Extensions.Configuration;

namespace Nameless.Mockers;

public class ConfigurationManagerMocker : MockerBase<IConfigurationManager> {
    public ConfigurationManagerMocker WithSection(string name, IConfigurationSection section) {
        InnerMock.Setup(mock => mock.GetSection(name))
                    .Returns(section);

        return this;
    }
}