using System.Globalization;
using Microsoft.Extensions.Logging.Abstractions;

namespace Nameless.Localization.Json.Infrastructure;

public class CultureProviderTests {
    private static CultureProvider CreateSut() => new(NullLogger<CultureProvider>.Instance);

    [Test]
    public void GetCurrentCulture_Returns_CultureInfo_From_CurrentUICulture() {
        // arrange
        Thread.CurrentThread.CurrentUICulture = new CultureInfo("es-ES");
        Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
        var sut = CreateSut();

        // act
        // First, tries to get culture from Thread.CurrentThread.CurrentUICulture.
        // If CurrentUICulture is empty, then tries Thread.CurrentThread.CurrentCulture
        var actual = sut.GetCurrentCulture();

        // assert
        Assert.That(actual.Name, Is.EqualTo("es-ES"));
    }

    [Test]
    public void GetCurrentCulture_Returns_CultureInfo_From_CurrentCulture() {
        // arrange
        Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
        Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR");
        var sut = CreateSut();

        // act
        // First, tries to get culture from Thread.CurrentThread.CurrentUICulture.
        // If CurrentUICulture is empty, then tries Thread.CurrentThread.CurrentCulture
        var actual = sut.GetCurrentCulture();

        // assert
        Assert.That(actual.Name, Is.EqualTo("fr-FR"));
    }

    [Test]
    public void GetCurrentCulture_Returns_Default_CultureInfo_en_US_If_CurrentThread_Does_Not_Provides() {
        // arrange
        Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
        Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
        var sut = CreateSut();

        // act
        // If both culture object return empty, get the default that is en-US
        var actual = sut.GetCurrentCulture();

        // assert
        Assert.That(actual.Name, Is.EqualTo("en-US"));
    }
}