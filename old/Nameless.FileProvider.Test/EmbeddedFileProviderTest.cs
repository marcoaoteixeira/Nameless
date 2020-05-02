using Nameless.FileProvider.Embedded;
using Nameless.IoC.Autofac;
using Nameless.IoC;
using Xunit;
using Nameless.FileProvider.Test.Fixtures;

namespace Nameless.FileProvider.Test {
    public class EmbeddedFileProviderTest {
        [Fact]
        public void Can_Create () {
            // arrange
            IFileProvider fileProvider;

            // act
            fileProvider = new EmbeddedFileProvider (typeof (EmbeddedFileProviderTest).Assembly);

            // assert
            Assert.NotNull (fileProvider);
        }

        [Fact]
        public void Can_Create_With_Autofac () {
            // arrange
            using (var root = new CompositionRoot ()) {
                // act
                root.Compose (new ServiceRegistrationBase[] {
                    new FileProviderServiceRegistration (),
                    new ServiceSampleServiceRegistration ()
                });

                root.StartUp ();
                var serviceSample = root.GetServiceResolver ().Get<IServiceSample> ();

                // assert
                Assert.NotNull (serviceSample);
            }
        }
    }
}
