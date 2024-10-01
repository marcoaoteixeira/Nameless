using System.Reflection;
using Nameless.Checklist.Web.Domain.Entities;
using Nameless.Checklist.Web.Domain.PostProcessors;
using Nameless.Checklist.Web.Domain.Repositories;
using Nameless.Checklist.Web.Domain.Repositories.Impl;
using Nameless.MediatR.Integration;
using Nameless.Validation.FluentValidation;
using Nameless.Web;
using Nameless.Web.Endpoints;
using Nameless.Web.Options;

namespace Nameless.Checklist.Web;

public static class EntryPoint {
    private static Assembly[] SupportAssemblies { get; } = [
        typeof(EntryPoint).Assembly
    ];

    public static void Main(string[] args) {
        var builder = WebApplication.CreateBuilder(args);

        builder.Configuration
               .AddJsonFile("AppSettings.json", optional: true, reloadOnChange: true)
               .AddJsonFile($"AppSettings.{builder.Environment.EnvironmentName}.json", optional: true)
               .AddEnvironmentVariables();

        builder.Logging
               .AddConfiguration(builder.Configuration.GetSection("Logging"))
               .AddConsole()
               .AddDebug();

        builder.Services
               .AddOptions()
               .Configure<SwaggerPageOptions>(builder.Configuration.GetSection(nameof(SwaggerPageOptions)))
               .Configure<JwtOptions>(builder.Configuration.GetSection(nameof(JwtOptions)));

        builder.Services
               .AddAutoMapper(SupportAssemblies);

        builder.Services
               .AddMediatR(opts => {
                   opts.RegisterServicesFromAssemblies(SupportAssemblies);
                   opts.AddOpenBehavior(typeof(ValidationPipelineBehavior<,>));
                   opts.AddRequestPostProcessor<CreateTodoItemRequestPostProcessor>();
               });

        builder.Services
               .AddSingleton<IRepository<ChecklistItem>>(_ => {
                   var path = typeof(EntryPoint).Assembly.GetDirectoryPath("App_Data/database.json");
                   return new TodoItemRepository(path);
               });

        builder.Services.AddJwtAuth(builder.Configuration);
        builder.Services.AddMinimalEndpoints(SupportAssemblies);
        builder.Services.AddApiVersioningDefault();
        builder.Services.AddSwagger(enableJwt: true);
        builder.Services.AddValidationService(SupportAssemblies);
        builder.Services.AddSystemClock();
        
        builder.Services.AddRouting();

        var app = builder.Build();

        app.UseRouting();
        app.UseMinimalEndpoints();
        app.UseSwaggerWithVersioning();
        app.UseJwtAuth();

        if (!app.Environment.IsDevelopment()) {
            // The default HSTS value is 30 days. You may want to change
            // this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
            app.UseExceptionHandler("/error");
        }
        app.UseHttpsRedirection();

        app.Run();
    }
}