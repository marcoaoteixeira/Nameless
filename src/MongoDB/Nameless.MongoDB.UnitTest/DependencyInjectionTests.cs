using Autofac;
using Nameless.MongoDB.DependencyInjection;
using Nameless.MongoDB.Impl;
using Nameless.MongoDB.Options;
using Nameless.Test.Utils;

namespace Nameless.MongoDB.UnitTest {
    public class DependencyInjectionTests {
        [Category(Categories.RunsOnDevMachine)]
        [Test(Description = "You'll need a local mongo instance or configure it to access a remote instance. See README file.")]
        public void Register_Resolve_MongoDB_Services() {
            // arrange
            var builder = new ContainerBuilder();
            builder.RegisterMongoDBModule();

            builder
                .RegisterInstance(new MongoOptions {
                    Database = "local"
                })
                .SingleInstance();

            using var container = builder.Build();

            // act
            var service = container.Resolve<IMongoCollectionProvider>();

            // assert
            Assert.Multiple(() => {
                Assert.That(service, Is.Not.Null);
                Assert.That(service, Is.InstanceOf<MongoCollectionProvider>());
            });
        }
    }
}
