using Autofac;
using Nameless.Validation.Abstractions;
using Nameless.Validation.FluentValidation.DependencyInjection;
using Nameless.Validation.FluentValidation.Impl;

namespace Nameless.Validation.FluentValidation.UnitTest {
    public class DependencyInjectionTests {
        [Test]
        public void Register_Resolve_Service() {
            // arrange
            var builder = new ContainerBuilder();
            builder.RegisterValidationModule();
            using var container = builder.Build();

            // act
            var sut = container.Resolve<IValidationService>();

            // assert
            Assert.Multiple(() => {
                Assert.That(sut, Is.Not.Null);
                Assert.That(sut, Is.InstanceOf<ValidationService>());
            });
        }
    }
}
