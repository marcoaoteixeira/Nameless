using Autofac;
using Nameless.Data.SQLServer.DependencyInjection;

namespace Nameless.Data.SQLServer {
    public class DependencyInjectionTests {
        [Test]
        public void Register_Resolve_Dependency_Injection() {
            // arrange
            var builder = new ContainerBuilder();
            builder.RegisterDataModule();
            using var container = builder.Build();

            // act
            var database = container.Resolve<IDatabase>();

            // assert
            Assert.Multiple(() => {
                Assert.That(database, Is.Not.Null);
                Assert.That(database, Is.InstanceOf<Database>());
            });
        }
    }
}
