using Autofac;

namespace Nameless.Lucene.UnitTests {

    public class LuceneModuleTests {

        [Test]
        public void LuceneModule_Correctly_Register_Services() {
            // arrange
            var builder = new ContainerBuilder();
            builder.RegisterInstance(new LuceneOptions {
                IndexesDirectoryName = typeof(LuceneModuleTests).Assembly.GetDirectoryPath("App_Data", "Lucene"),
            });
            builder.RegisterModule<LuceneModule>();

            var container = builder.Build();

            // act
            var indexProvider = container.Resolve<IIndexProvider>();

            // assert
            indexProvider.Should().NotBeNull();
        }
    }
}
