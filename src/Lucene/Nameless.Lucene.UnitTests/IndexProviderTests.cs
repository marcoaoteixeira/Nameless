using Autofac;
using Microsoft.Extensions.Options;
using Nameless.Infrastructure;
using NSubstitute;

namespace Nameless.Lucene.UnitTests
{

    public class IndexProviderTests {

        private IContainer _container;

        [OneTimeSetUp]
        public void OneTimeSetUp() {
            var applicationContext = Substitute.For<IApplicationContext>();
            applicationContext.DataDirectoryPath.Returns(typeof(IndexProviderTests).Assembly.GetDirectoryPath("App_Data"));

            var builder = new ContainerBuilder();
            builder.RegisterInstance(applicationContext).As<IApplicationContext>();
            builder.RegisterInstance(Options.Create(LuceneOptions.Default));
            builder.RegisterModule<LuceneModule>();

            _container = builder.Build();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown() {
            _container.Dispose();
        }

        [Test]
        public void Can_Create_A_Index() {
            // arrange
            var indexName = Guid.NewGuid().ToString("N").ToUpper();
            var indexProvider = _container.Resolve<IIndexProvider>();

            // act
            var index = indexProvider.GetOrCreate(indexName);

            // assert
            index.Should().NotBeNull();
        }
    }
}
