using Autofac;
using Microsoft.Extensions.Options;

namespace Nameless.Lucene.UnitTests {

    public class LuceneModuleTests {

        [Test]
        public void LuceneModule_Correctly_Register_Services() {
            // arrange
            var builder = new ContainerBuilder();
            var options = Options.Create(new LuceneOptions {
                IndexesDirectoryName = typeof(LuceneModuleTests).Assembly.GetDirectoryPath("App_Data", "Lucene"),
            });
            builder.RegisterInstance(NullApplicationContext.Instance);
            builder.RegisterInstance(options);
            builder.RegisterModule<LuceneModule>();

            var container = builder.Build();

            // act
            var indexProvider = container.Resolve<IIndexProvider>();

            // assert
            indexProvider.Should().NotBeNull();
        }
    }
}
