﻿using System.Globalization;
using Microsoft.Extensions.Logging.Abstractions;

namespace Nameless.Localization.Json.Infrastructure;

public class CultureProviderTests {
    private static CultureProvider CreateSut() {
        return new CultureProvider(NullLogger<CultureProvider>.Instance);
    }

    [Fact]
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
        Assert.Equal("es-ES", actual.Name);
    }

    [Fact]
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
        Assert.Equal("fr-FR", actual.Name);
    }

    [Fact]
    public void GetCurrentCulture_Returns_Default_CultureInfo_en_US_If_CurrentThread_Does_Not_Provides() {
        // arrange
        Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
        Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
        var sut = CreateSut();

        // act
        // If both culture object return empty, get the default that is en-US
        var actual = sut.GetCurrentCulture();

        // assert
        Assert.Equal("en-US", actual.Name);
    }
}