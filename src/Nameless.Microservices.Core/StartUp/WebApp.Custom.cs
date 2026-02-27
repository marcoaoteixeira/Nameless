using Microsoft.AspNetCore.Builder;

namespace Nameless.Microservices.StartUp;

public static partial class WebAppExtensions {
    extension(WebApplicationBuilder self) {
        public WebApplicationBuilder CustomConfig(Action<WebApplicationBuilder> configure) {
            configure(self);

            return self;
        }
    }

    extension(WebApplication self) {
        public WebApplication UseCustom(Action<WebApplication> configure) {
            configure(self);

            return self;
        }
    }
}
