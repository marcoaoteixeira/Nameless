using Microsoft.Extensions.Hosting;
using Nameless.Infrastructure;

namespace Nameless.WebApplication.Infrastructure {

    public sealed class ApplicationContext : IApplicationContext {

        #region Public Constructors

        public ApplicationContext(IHostEnvironment hostEnvironment) {
            Prevent.Null(hostEnvironment, nameof(hostEnvironment));

            EnvironmentName = hostEnvironment.EnvironmentName;
            ApplicationName = hostEnvironment.ApplicationName;
            BasePath = typeof(ApplicationContext).Assembly.GetDirectoryPath();
            DataDirectoryPath = Path.Combine(BasePath, "App_Data");
        }

        #endregion

        #region IApplicationContext Members

        public string EnvironmentName { get; }
        public string ApplicationName { get; }
        public string BasePath { get; }
        public string DataDirectoryPath { get; }

        #endregion

    }
}
