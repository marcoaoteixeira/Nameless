using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Nameless.AspNetCore.Versioning;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Nameless.WebApplication.Web {

    public partial class StartUp {

        #region Private Static Methods

        private static void ConfigureVersioning(IServiceCollection services) {
            services.AddApiVersioning(opts => {
                opts.ReportApiVersions = true;
                opts.AssumeDefaultVersionWhenUnspecified = true;
                opts.DefaultApiVersion = new ApiVersion(1, 0);
            });

            services.AddVersionedApiExplorer(opts => {
                // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
                // note: the specified format code will format the version as "'v'major[.minor][-status]"
                opts.GroupNameFormat = "'v'VVV";

                // note: this option is only necessary when versioning by url segment. the SubstitutionFormat
                // can also be used to control the format of the API version in route templates
                opts.SubstituteApiVersionInUrl = true;
            });

            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, SwaggerGenConfigureOptions>();
        }

        #endregion
    }
}
