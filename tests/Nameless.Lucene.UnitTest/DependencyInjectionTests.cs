﻿using Microsoft.Extensions.DependencyInjection;
using Moq;
using Nameless.Infrastructure;
using Nameless.Lucene.Impl;

namespace Nameless.Lucene;

public class DependencyInjectionTests {
    [Test]
    public void Register_Resolve_Service() {
        // arrange
        var services = new ServiceCollection();
        services.AddLucene();

        // We need an IApplicationContext
        var applicationContextMock = new Mock<IApplicationContext>();
        services
            .AddSingleton(applicationContextMock.Object);

        using var provider = services.BuildServiceProvider();

        // act
        var service = provider.GetService<IIndexProvider>();

        // assert
        Assert.That(service, Is.InstanceOf<IndexProvider>());
    }
}