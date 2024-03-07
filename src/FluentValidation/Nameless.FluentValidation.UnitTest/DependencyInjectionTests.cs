using Autofac;
using Nameless.FluentValidation.DependencyInjection;
using Nameless.FluentValidation.Impl;

namespace Nameless.FluentValidation {
    public class DependencyInjectionTests {
        [Test]
        public void Register_Resolve_Service() {
            // arrange
            var builder = new ContainerBuilder();
            builder.RegisterFluentValidationModule();
            using var container = builder.Build();

            // act
            var service = container.Resolve<IValidatorManager>();

            // assert
            Assert.Multiple(() => {
                Assert.That(service, Is.Not.Null);
                Assert.That(service, Is.InstanceOf<ValidatorManager>());
            });
        }
    }
}
