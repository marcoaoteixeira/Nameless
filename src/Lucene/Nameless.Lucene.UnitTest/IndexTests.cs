using Autofac;
using Moq;
using Nameless.Infrastructure;
using Nameless.Lucene.DependencyInjection;
using Nameless.Lucene.Impl;
using Nameless.Test.Utils;

namespace Nameless.Lucene {
    public class IndexTests {
        private static readonly string IndexDirectoryPath = typeof(IndexTests).Assembly.GetDirectoryPath("Output");

        private static IContainer CreateContainer() {
            var builder = new ContainerBuilder();
            builder.RegisterLuceneModule();

            var applicationContextMock = new Mock<IApplicationContext>();
            applicationContextMock
                .Setup(mock => mock.ApplicationDataFolderPath)
                .Returns(IndexDirectoryPath);
            builder
                .RegisterInstance(applicationContextMock.Object)
                .As<IApplicationContext>()
                .SingleInstance();

            return builder.Build();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown() {
            try { Directory.Delete(IndexDirectoryPath, recursive: true); }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
        }

        [Category(Categories.RUNS_ON_DEV_MACHINE)]
        [Test]
        public void Should_Create_Instance_Of_Index_Class() {
            using var container = CreateContainer();
            const string INDEX_NAME = "32b52ab3-7069-4fae-ac15-d9f3442db19b";

            var indexManager = container.Resolve<IIndexManager>();
            var indexA = indexManager.GetOrCreate(INDEX_NAME);
            var indexB = indexManager.GetOrCreate(INDEX_NAME);

            Assert.Multiple(() => {
                Assert.That(indexA, Is.Not.Null);
                Assert.That(indexB, Is.Not.Null);
                Assert.That(indexA.GetHashCode(), Is.EqualTo(indexB.GetHashCode()));
            });
        }

        [Category(Categories.RUNS_ON_DEV_MACHINE)]
        [Test]
        public void StoreDocument_Should_Create_A_New_Document_In_Index() {
            using var container = CreateContainer();
            const string INDEX_NAME = "d39ff2d3-7d84-4d41-99e6-e096754d14be";

            var indexManager = container.Resolve<IIndexManager>();
            var index = indexManager.GetOrCreate(INDEX_NAME);

            var loremIpsumFilePath = typeof(IndexTests).Assembly.GetDirectoryPath("Resources", "LoremIpsum.txt");
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

        [Category(Categories.RUNS_ON_DEV_MACHINE)]
        [Test]
        public void CreateSearchBuilder_Should_Return_Search_Service_And_Find_Document() {
            using var container = CreateContainer();
            const string INDEX_NAME = "82b3dcd7-85c1-4c73-8f49-c54ae82ab2f8";

            var indexManager = container.Resolve<IIndexManager>();
            var index = indexManager.GetOrCreate(INDEX_NAME);

            var loremIpsumFilePath = typeof(IndexTests).Assembly.GetDirectoryPath("Resources", "LoremIpsum.txt");
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

        [Category(Categories.RUNS_ON_DEV_MACHINE)]
        [Test]
        public void Multiple_Documents_Store_Different_Moments() {
            // setup
            using var container = CreateContainer();
            const string INDEX_NAME = "e112b156-ecfc-4fb9-90db-9675bc61b3ba";
            const string FIELD_NAME = "Content";

            // arrange
            var indexManager = container.Resolve<IIndexManager>();
            var index = indexManager.GetOrCreate(INDEX_NAME);
            
            // act 1
            var filePathForText001 = typeof(IndexTests).Assembly.GetDirectoryPath("Resources", "text_001.txt");
            var contentText001 = File.ReadAllText(filePathForText001);

            var documentText001 = index
                                  .NewDocument(Guid.NewGuid()
                                                   .ToString("N"))
                                  .Set(FIELD_NAME, contentText001, FieldOptions.Analyze | FieldOptions.Store);
            index.StoreDocuments([documentText001]);

            var searchBuilderText001 = index.CreateSearchBuilder();

            searchBuilderText001.WithField(FIELD_NAME, "vibrant", useWildcard: false);

            var resultText001 = searchBuilderText001
                                .Search()
                                .Select(hit => hit.GetString(FIELD_NAME))
                                .ToArray();

            // act 2
            var filePathForText002 = typeof(IndexTests).Assembly.GetDirectoryPath("Resources", "text_002.txt");
            var contentText002 = File.ReadAllText(filePathForText002);

            var documentText002 = index
                                  .NewDocument(Guid.NewGuid()
                                                   .ToString("N"))
                                  .Set(FIELD_NAME, contentText002, FieldOptions.Analyze | FieldOptions.Store);
            index.StoreDocuments([documentText002]);

            var searchBuilderText002 = index.CreateSearchBuilder();

            searchBuilderText002.WithField(FIELD_NAME, "ideas", useWildcard: false);

            var resultText002 = searchBuilderText002
                                .Search()
                                .Select(hit => hit.GetString(FIELD_NAME))
                                .ToArray();

            Assert.Multiple(() => {
                Assert.That(resultText001, Is.Not.Empty);
                Assert.That(resultText002, Is.Not.Empty);
            });
        }
    }
}
