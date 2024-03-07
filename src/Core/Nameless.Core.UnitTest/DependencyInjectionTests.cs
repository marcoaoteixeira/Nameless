using Autofac;
using Nameless.DependencyInjection;
using Nameless.Services;

namespace Nameless {
    public class DependencyInjectionTests {
        [Test]
        public void Register_And_Resolve_Services() {
            // arrange
            var builder = new ContainerBuilder();
            builder.RegisterCoreModule();
            using var container = builder.Build();

            // act
            var clock = container.Resolve<IClock>();
            var xmlSchemaValidator = container.Resolve<IXmlSchemaValidator>();
            var pluralizationRuleProvider = container.Resolve<IPluralizationRuleProvider>();

            // assert
            Assert.Multiple(() => {
                Assert.That(clock, Is.Not.Null);
                Assert.That(xmlSchemaValidator, Is.Not.Null);
                Assert.That(pluralizationRuleProvider, Is.Not.Null);
            });
        }
    }
}
