using Nameless.Hosting;
using Nameless.Hosting.Features.Auth;
using Nameless.Sample.WebApp.AutoGenCode;
using Nameless.Web.Hosting;
using Nameless.Web.Hosting.Features.Scalar;
using Nameless.Web.Scalar.Auth.OAuth;

namespace Nameless.Sample.WebApp;

public class EntryPoint {
    public static void Main(string[] args) {
        WebHostFactory.CreateDefault(args, [typeof(EntryPoint).Assembly], ConfigureFeatures)
                      .Run();
    }

    private static void ConfigureFeatures(HostFeatureCollection features) {
        features.Add(new OAuthAccessTokenProviderHostFeature());
        features.Add(new AppHostFeature());
        features.Get<ScalarWebHostFeature>().Configure(scalar => scalar.WithScalarOptionsConfiguration(
            (scalarOpts, ctx) => scalarOpts.SetOAuthHttpSecurity(ctx)
        ));
    }
}

internal class AppHostFeature : HostFeature, IWebHostFeature {
    public override string Name => "Sample Application Host Feature";

    public override void Register(IHostApplicationBuilder builder, HostFeatureContext context) {
        builder.Services.RegisterAutoEndpoints();
    }

    public void Use(WebApplication app, HostFeatureContext context) {
        app.MapAutoEndpoints();
    }
}