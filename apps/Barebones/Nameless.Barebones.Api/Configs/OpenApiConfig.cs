﻿using Nameless.Barebones.Application;
using Scalar.AspNetCore;

namespace Nameless.Barebones.Api.Configs;

public static class OpenApiConfig {
    public static WebApplication UseOpenApi(this WebApplication self) {
        if (!self.Environment.IsDevelopment()) {
            return self;
        }

        self.MapOpenApi()
            .CacheOutput(OutputCachePolicy.OpenApiDocumentation.PolicyName);

        self.MapScalarApiReference(options => {
            options.WithTitle(self.Environment.ApplicationName)
                   .WithTheme(ScalarTheme.BluePlanet)
                   .WithDefaultHttpClient(ScalarTarget.Shell, ScalarClient.Curl);
        });

        return self;
    }
}
