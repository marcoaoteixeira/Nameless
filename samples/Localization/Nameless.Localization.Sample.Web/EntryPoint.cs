using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.FileProviders;
using Nameless.Localization.Json;
using Nameless.Localization.Sample.Web.Components;

namespace Nameless.Localization.Sample.Web;
public class EntryPoint {
    public static void Main(string[] args) {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        ConfigureLocalization(builder);

        builder.Services
               .AddRazorComponents()
               .AddInteractiveServerComponents();

        builder.Services
               .AddHttpContextAccessor();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment()) {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseStaticFiles();
        app.UseAntiforgery();

        app.MapRazorComponents<App>()
           .AddInteractiveServerRenderMode();

        app.Run();
    }

    private static void ConfigureLocalization(WebApplicationBuilder builder) {
        // We will need a file provider to load our translation files.
        builder.Services
               .AddSingleton<IFileProvider>(_ => new PhysicalFileProvider(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data")));

        // Configure localization to use HEADERS to choose what
        // language to display.
        var cultures = new[] { "pt-BR", "en-US", "fr-FR" };
        builder.Services
               .Configure<RequestLocalizationOptions>(opts => {
                   opts.SetDefaultCulture(cultures[0])
                       .AddSupportedCultures(cultures)
                       .AddSupportedUICultures(cultures);

                   opts.ApplyCurrentCultureToResponseHeaders = true;
                   opts.RequestCultureProviders.Clear();
                   opts.RequestCultureProviders.Add(new AcceptLanguageHeaderRequestCultureProvider());
               });

        // Add our localization services.
        builder.Services
               .RegisterJsonLocalization(builder.Configuration.GetSection("Localization")); }
}
