using System.Reflection;

namespace Nameless.Validation.FluentValidation;

public class ValidationRegistrationSettingsTests {
    [Fact]
    public void WhenSettingAssembliesProperty_ThenEnsureCanRetrieveIt() {
        // arrange
        Assembly[] assemblies = [typeof(ValidationRegistrationSettingsTests).Assembly];

        // act
        var sut = new ValidationRegistrationSettings().IncludeAssemblies(assemblies);

        // assert
        Assert.NotEmpty(sut.Assemblies);
    }
}