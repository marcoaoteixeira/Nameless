﻿using Moq;
using Nameless.Infrastructure;

namespace Nameless.FileStorage.System.UnitTesting {

    public class FileStorageTest {

        [Test]
        public async Task GetFileAsync_Retrieves_File() {
            var fileStorage = new FileStorageImpl(NullApplicationContext.Instance);

            var file = await fileStorage.GetFileAsync("Temp.txt");

            Assert.That(file, Is.Not.Null);
        }

        [Test]
        public async Task GetFileAsync_Retrieves_Existent_File() {
            var appContext = new Mock<IApplicationContext>();

            var directoryPath = GetType().Assembly.GetDirectoryPath();

            Console.WriteLine($"[GetFileAsync_Retrieves_Existent_File] Directory path: {directoryPath}");

            appContext
                .Setup(_ => _.BasePath)
                .Returns(GetType().Assembly.GetDirectoryPath());
            appContext
                .Setup(_ => _.DataDirectoryPath)
                .Returns(GetType().Assembly.GetDirectoryPath());

            var fileStorage = new FileStorageImpl(appContext.Object);

            var fileName = Path.GetFileName(typeof(FileStorageTest).Assembly.Location);
            var file = await fileStorage.GetFileAsync(fileName);

            Assert.That(file, Is.Not.Null);
            Assert.That(file.Exists, Is.True);
        }

        [Test]
        public async Task Read_File_From_Storage() {
            // arrange
            var expected = "This is a Test!";
            var fileStorage = new FileStorageImpl(NullApplicationContext.Instance);

            // act
            var file = await fileStorage.GetFileAsync("Content\\TextFile.txt");
            var content = (await file.OpenAsync()).ToText();

            // assert
            Assert.Multiple(() => {
                Assert.That(file, Is.Not.Null);
                Assert.That(file.Exists, Is.True);
                Assert.That(content, Is.EqualTo(expected));
            });
        }
    }
}