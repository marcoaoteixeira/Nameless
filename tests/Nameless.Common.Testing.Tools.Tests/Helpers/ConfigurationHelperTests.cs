using Microsoft.Extensions.Configuration;
using Nameless.Testing.Tools.Attributes;
using Nameless.Testing.Tools.Helpers;

namespace Nameless.Common.Testing.Tools.Helpers;

[UnitTest]
public class ConfigurationHelperTests
{
    [Fact]
    public void WhenInitializeConfiguration_ThenCanReadAppSettingsSection()
    {
        // arrange
        const string SectionName = nameof(ConfigurationHelper);
        const string Message = "It works!";
        var configuration = ConfigurationHelper.CreateConfiguration();

        // act
        var section = configuration.GetSection(SectionName);
        var value = section.GetValue<string>(nameof(Message));

        // assert
        Assert.Equal(Message, value);
    }
}