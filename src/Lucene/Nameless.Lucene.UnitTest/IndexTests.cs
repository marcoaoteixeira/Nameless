using Autofac;
using Moq;
using Nameless.Infrastructure;
using Nameless.Lucene.DependencyInjection;
using Nameless.Lucene.Impl;
using Nameless.Test.Utils;

namespace Nameless.Lucene {
    public class IndexTests {
        private static IContainer CreateContainer() {
            var builder = new ContainerBuilder();
            builder.RegisterLuceneModule();

            var applicationContextMock = new Mock<IApplicationContext>();
            applicationContextMock
                .Setup(mock => mock.ApplicationDataFolderPath)
                .Returns(Path.GetTempPath());
            builder
                .RegisterInstance(applicationContextMock.Object)
                .As<IApplicationContext>()
                .SingleInstance();

            return builder.Build();
        }

        [Category(Categories.RunsOnDevMachine)]
        [Test]
        public void Should_Create_Instance_Of_Index_Class() {
            using var container = CreateContainer();

            var indexManager = container.Resolve<IIndexManager>();
            var indexA = indexManager.GetOrCreate("temporary");
            var indexB = indexManager.GetOrCreate("temporary");

            Assert.Multiple(() => {
                Assert.That(indexA, Is.Not.Null);
                Assert.That(indexB, Is.Not.Null);
                Assert.That(indexA.GetHashCode(), Is.EqualTo(indexB.GetHashCode()));
            });
        }

        [Category(Categories.RunsOnDevMachine)]
        [Test]
        public void StoreDocument_Should_Create_A_New_Document_In_Index() {
            using var container = CreateContainer();

            var indexManager = container.Resolve<IIndexManager>();
            var index = indexManager.GetOrCreate("temporary");
            var loremIpsumFilePath = typeof(IndexTests).Assembly.GetDirectoryPath("LoremIpsum.txt");
            var loremIpsum = File.ReadAllText(loremIpsumFilePath);

            var document = new Document("146ef344-ae25-4346-b07a-7da8f418a26f")
                .Set("Name", "Test User", FieldOptions.Store)
                .Set("Email", "test_user@test.com", FieldOptions.Store)
                .Set("Birthday", DateTime.Now.Date, FieldOptions.Store)
                .Set("Weight", 75d, FieldOptions.Store)
                .Set("Married", true, FieldOptions.Store)
                .Set("Age", 50, FieldOptions.Store)
                .Set("Content", loremIpsum, FieldOptions.Analyze | FieldOptions.Store);

            index.StoreDocuments([document]);

            Assert.That(index, Is.Not.Null);
        }

        [Category(Categories.RunsOnDevMachine)]
        [Test]
        public void CreateSearchBuilder_Should_Return_Search_Service_And_Find_Document() {
            using var container = CreateContainer();

            var indexManager = container.Resolve<IIndexManager>();
            var index = indexManager.GetOrCreate("temporary");

            var searcher = index.CreateSearchBuilder();
            var tokens = new[] {
                "ullamcorper",
                "ultrices",
                "Morbi",
                "Sbrubles"
            };

            searcher
                .WithField("Content", tokens[0], useWildcard: false)
                .Mandatory();
            searcher
                .WithField("Content", tokens[1], useWildcard: false)
                .Mandatory();
            searcher
                .WithField("Content", tokens[2], useWildcard: false)
                .ExactMatch();
            searcher
                .WithField("Content", tokens[3], useWildcard: false)
                .ExactMatch();

            var result = searcher.Search();

            Assert.Multiple(() => {
                Assert.That(index, Is.Not.Null);
                Assert.That(result, Is.Not.Empty);
            });
        }
    }
}
