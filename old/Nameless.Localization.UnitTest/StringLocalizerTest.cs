using System.IO;
using Moq;
using Nameless.FileProvider;
using Nameless.Localization.Json;
using Xunit;

namespace Nameless.Localization.UnitTest
{
    // public class StringLocalizerTest {
    //     [Fact]
    //     public void Can_Create () {
    //         // arrange
    //         var fileProvider = new Mock<IFileProvider> ();
    //         var file = new Mock<IFile> ();
    //         fileProvider
    //             .Setup (_ => _.GetFile (It.IsAny<string> ()))
    //             .Returns (file.Object);
    //         file
    //             .Setup (_ => _.GetStream ())
    //             .Returns (Stream.Null);

    //         IStringLocalization localizer;

    //         // act
    //         localizer = new StringLocalizer ("Test", "pt-BR", Plural);

    //         // assert
    //         Assert.NotNull (localizer);
    //     }

    //     [Fact]
    //     public void Can_Load_Localization_From_Resource () {
    //         // arrange
    //         var json = @"{
    //             'This is a test': [
    //                 'Isso é um teste'
    //             ]
    //         }";
    //         var fileProvider = new Mock<IFileProvider> ();
    //         var file = new Mock<IFile> ();
    //         fileProvider
    //             .Setup (_ => _.GetFile (It.IsAny<string> ()))
    //             .Returns (file.Object);
    //         file
    //             .Setup (_ => _.GetStream ())
    //             .Returns (json.ToStream ());

    //         IStringLocalization localizer;

    //         // act
    //         localizer = new StringLocalization (fileProvider.Object, "Test", "pt-BR", Plural);

    //         // assert
    //         Assert.NotNull (localizer);
    //         Assert.NotNull (localizer.Get ("This is a test"));
    //         Assert.Equal ("Isso é um teste", localizer.Get ("This is a test"));
    //     }

    //     [Fact]
    //     public void Pluralization_From_Resource () {
    //         // arrange
    //         var json = @"{
    //             'Total of records': [
    //                 'There are no records',
    //                 'There are more than one records',
    //                 'There are thousands records'
    //             ]
    //         }";
    //         var fileProvider = new Mock<IFileProvider> ();
    //         var file = new Mock<IFile> ();
    //         fileProvider
    //             .Setup (_ => _.GetFile (It.IsAny<string> ()))
    //             .Returns (file.Object);
    //         file
    //             .Setup (_ => _.GetStream ())
    //             .Returns (json.ToStream ());

    //         IStringLocalization localizer;

    //         // act
    //         localizer = new StringLocalization (fileProvider.Object, "Test", "pt-BR", count => {
    //             if (count <= 0) { return 0; }

    //             if (count > 0 && count < 10) { return 1; }

    //             return 2;
    //         });

    //         // assert
    //         Assert.NotNull (localizer);
    //         Assert.NotNull (localizer.Get ("Total of records"));
    //         Assert.Equal ("There are no records", localizer.Get ("Total of records"));
    //         Assert.Equal ("There are more than one records", localizer.Get ("Total of records", 8));
    //         Assert.Equal ("There are thousands records", localizer.Get ("Total of records", 58));
    //     }
    // }
}