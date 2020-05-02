using Autofac;
using Nameless.FileProvider.Embedded;
using Nameless.IoC.Autofac;

namespace Nameless.FileProvider.Test.Fixtures {
    public interface IServiceSample {
        string GetText (string path, string cultureName = null);
    }
    public class ServiceSample : IServiceSample {
        private readonly IFileProvider _fileProvider;

        public ServiceSample (IFileProvider fileProvider) {
            _fileProvider = fileProvider;
        } 

        public string GetText (string path, string cultureName = null) {
            return _fileProvider.GetFile (path, cultureName).GetStream ().ToText ();
        }
    }

    public class ServiceSampleServiceRegistration : ServiceRegistrationBase {
        protected override void Load (ContainerBuilder builder) {
            builder
                .RegisterType<ServiceSample> ()
                .As<IServiceSample> ();

            base.Load (builder);
        }
    }
}
