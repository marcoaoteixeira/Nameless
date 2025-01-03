﻿using Microsoft.Extensions.Configuration;

namespace Nameless.Mockers;

public class ConfigurationSectionMocker : MockerBase<IConfigurationSection> {
    public ConfigurationSectionMocker WithKey(string key) {
        Mock.Setup(mock => mock.Key)
                    .Returns(key);

        return this;
    }

    public ConfigurationSectionMocker WithValue(string value) {
        Mock.Setup(mock => mock.Value)
                    .Returns(value);

        return this;
    }
}