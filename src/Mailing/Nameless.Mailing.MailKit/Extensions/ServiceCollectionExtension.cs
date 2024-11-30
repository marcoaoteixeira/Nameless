﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nameless.Mailing.MailKit.Options;

namespace Nameless.Mailing.MailKit;

public static class ServiceCollectionExtension {
    public static IServiceCollection AddMailkitMailing(this IServiceCollection self, Action<MailingOptions> configure)
        => Prevent.Argument
                  .Null(self)
                  .Configure(configure)
                  .RegisterMailingServices();

    public static IServiceCollection AddMailkitMailing(this IServiceCollection self, IConfigurationSection mailingConfigSection)
        => Prevent.Argument
                  .Null(self)
                  .Configure<MailingOptions>(mailingConfigSection)
                  .RegisterMailingServices();

    private static IServiceCollection RegisterMailingServices(this IServiceCollection self)
        => self.AddSingleton<ISmtpClientFactory, SmtpClientFactory>()
               .AddSingleton<IEmailDispatcher, EmailDispatcher>();
}