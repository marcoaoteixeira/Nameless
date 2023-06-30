using Autofac;

namespace Nameless.Microservice {
    public partial class StartUp {
        #region Public Methods

        // ConfigureContainer is where you can register things directly
        // with Autofac. This runs after ConfigureServices so the things
        // here will override registrations made in ConfigureServices.
        // Don't build the container; that gets done for you by the factory.
        public void ConfigureContainer(ContainerBuilder builder) {

        }

        #endregion
    }
}