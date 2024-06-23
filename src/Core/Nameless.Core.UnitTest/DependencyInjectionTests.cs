using Microsoft.Extensions.DependencyInjection;
using Nameless.Services;
using Nameless.Services.Impl;

namespace Nameless {
    public class DependencyInjectionTests {
        [Test]
        public void Register_And_Resolve_Services() {
            // arrange
            var services = new ServiceCollection();
            services
                .RegisterClockService()
                .RegisterPluralizationRuleProvider()
                .RegisterXmlSchemaValidator();
            using var provider = services.BuildServiceProvider();

            // act
            var clockService = provider.GetService<IClockService>();
            var xmlSchemaValidator = provider.GetService<IXmlSchemaValidator>();
            var pluralizationRuleProvider = provider.GetService<IPluralizationRuleProvider>();

            // assert
            Assert.Multiple(() => {
                Assert.That(clockService, Is.InstanceOf<SystemClockService>());
                Assert.That(xmlSchemaValidator, Is.InstanceOf<XmlSchemaValidator>());
                Assert.That(pluralizationRuleProvider, Is.InstanceOf<PluralizationRuleProvider>());
            });
        }
    }
}
