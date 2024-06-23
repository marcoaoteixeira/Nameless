using Microsoft.Extensions.DependencyInjection;
using Nameless.Validation.Abstractions;
using Nameless.Validation.FluentValidation.Impl;

namespace Nameless.Validation.FluentValidation.UnitTest {
    public class DependencyInjectionTests {
        [Test]
        public void Register_Resolve_Service() {
            // arrange
            var services = new ServiceCollection();
            services.RegisterValidationService();
            using var container = services.BuildServiceProvider();

            // act
            var sut = container.GetService<IValidationService>();

            // assert
            Assert.That(sut, Is.InstanceOf<ValidationService>());
        }
    }
}
