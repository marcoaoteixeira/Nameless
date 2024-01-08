using Autofac;
using Nameless.Lucene.DependencyInjection;
using Nameless.Lucene.Impl;

namespace Nameless.Lucene {
    public class IntegrationTests {
        [Test, Ignore("Just for local machine")]
        public void Integration_Create_Instance() {
            var builder = new ContainerBuilder();
            builder.RegisterLuceneModule();
            using var container = builder.Build();

            var indexManager = container.Resolve<IIndexManager>();
            var indexA = indexManager.GetOrCreate("temporary");
            var indexB = indexManager.GetOrCreate("temporary");

            Assert.Multiple(() => {
                Assert.That(indexA, Is.Not.Null);
                Assert.That(indexB, Is.Not.Null);
                Assert.That(indexA.GetHashCode(), Is.EqualTo(indexB.GetHashCode()));
            });
        }

        [Test, Ignore("Just for local machine")]
        public void Integration_Store_Document() {
            var builder = new ContainerBuilder();
            builder.RegisterLuceneModule();
            using var container = builder.Build();

            var indexManager = container.Resolve<IIndexManager>();
            var index = indexManager.GetOrCreate("temporary");
            
            var document = new Document("146ef344-ae25-4346-b07a-7da8f418a26f")
                .Set("Name", "Test User", FieldOptions.Store)
                .Set("Email", "test_user@test.com", FieldOptions.Store)
                .Set("Birthday", DateTime.Now.Date, FieldOptions.Store)
                .Set("Weight", 75d, FieldOptions.Store)
                .Set("Married", true, FieldOptions.Store)
                .Set("Age", 50, FieldOptions.Store);

            index.StoreDocuments([document]);

            Assert.Multiple(() => {
                Assert.That(index, Is.Not.Null);
            });
        }

        [Test, Ignore("Just for local machine")]
        public void Integration_Search_Document() {
            var builder = new ContainerBuilder();
            builder.RegisterLuceneModule();
            using var container = builder.Build();

            var indexManager = container.Resolve<IIndexManager>();
            var index = indexManager.GetOrCreate("temporary");

            var searcher = index.CreateSearchBuilder();

            searcher.WithField("Name", "*User*", useWildcard: true);

            var result = searcher.Search();

            Assert.Multiple(() => {
                Assert.That(index, Is.Not.Null);
                Assert.That(result, Is.Not.Empty);
            });
        }
    }
}
