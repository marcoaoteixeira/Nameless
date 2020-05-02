using Nameless.IoC.Autofac;
using Nameless.Orm.NHibernate.Extra;
using Xunit;
using Nameless.IoC;
using Nameless.Orm.NHibernate.Test.Fixtures;
using System;
using System.IO;

namespace Nameless.Orm.NHibernate.Test {
    public class CompleteTest {
        [Fact]
        public async void Complete () {
            // arrange
            var path = Path.Combine (typeof (CompleteTest).Assembly.GetDirectoryPath (), "database.s3db");
            var compositionRoot = new CompositionRoot ();
            compositionRoot.Compose (new NHibernateServiceRegistration (typeof (CompleteTest).Assembly) {
                Settings = new NHibernateSettings {
                    ConnectionString = $"Data Source={path}; Version=3; BinaryGuid=False;"
                }
            });
            compositionRoot.StartUp ();

            // act
            var repository = compositionRoot.GetServiceResolver ().Get<IRepository> ();

            var user = new User {
                Name = "test",
                Email = "test@test.com",
                Password = "test",
                Avatar = "test"
            };

            await repository.SaveAsync (new[] { user });

            var result = await repository.FindOneAsync <User>(user.ID);

            // arrange
            Assert.NotNull (repository);
            Assert.NotNull (result);
            Assert.Equal ("test", user.Name);
            Assert.Equal ("test", user.Avatar);

            compositionRoot.TearDown ();
        }
    }
}
