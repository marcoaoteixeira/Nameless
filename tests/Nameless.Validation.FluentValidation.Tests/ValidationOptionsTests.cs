using System.Reflection;

namespace Nameless.Validation.FluentValidation;

public class ValidationOptionsTests {
    [Fact]
    public void WhenSettingAssembliesProperty_ThenEnsureCanRetrieveIt() {
        // arrange
        Assembly[] assemblies = [typeof(ValidationOptionsTests).Assembly];

        // act
        var sut = new ValidationOptions {
            Assemblies = assemblies
        };

        // assert
        Assert.NotEmpty(sut.Assemblies);
    }
}
