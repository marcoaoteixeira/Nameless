using System;
using System.Linq;
using System.Security.Claims;
using Nameless.AspNetCore.Identity.Stores;
using Nameless.AspNetCore.Identity.Stores.NHibernate.Mappings;
using Nameless.NHibernate.Support;
using Xunit;

namespace Nameless.AspNetCore.Identity.NHibernate.Test {
    public class UserStoreTest {
        private readonly Type[] MappingTypes = new [] {
            typeof (RoleClaimClassMapping),
            typeof (RoleClassMapping),
            typeof (UserClaimClassMapping),
            typeof (UserClassMapping),
            typeof (UserInRoleClassMapping),
            typeof (UserLoginClassMapping),
            typeof (UserTokenClassMapping)
        };

        [Fact]
        public void Test1 () {
            // As we're using SQLite in memory, when the connection
            // with the database is closed, all the schema is gone as well.
            // So, we need to maintain the session open as long as we needed
            var source = new SessionSource (mappingTypes: MappingTypes);
            var session = source.CreateSession ();

            var userID = Guid.NewGuid ().ToString ();
            var complement = userID.Replace ("-", string.Empty).Replace (" ", string.Empty);

            IdentityHelper.CreateUser (session, id : userID);

            session.Flush ();

            var user = session.Query<User> ().SingleOrDefault (_ => _.ID == userID);

            Assert.NotNull (user);
            Assert.Equal ($"USER_{complement}", user.NormalizedUserName);

            session.Dispose ();
            source.Dispose ();
        }

        [Fact]
        public async void GetUsersFromClaimAsync_GetUsers_ReturnsUsers () {
            using (var source = new SessionSource (mappingTypes: MappingTypes))
            using (var session = source.CreateSession ()) {
                // arrange
                var userID = Guid.NewGuid ().ToString ();
                var complement = userID.Replace ("-", string.Empty).Replace (" ", string.Empty);

                IdentityHelper.CreateUser (session, id : userID);
                IdentityHelper.CreateUserClaim (session, userID);

                session.Flush ();

                var userStore = new UserStore (session);

                // act
                var users = await userStore.GetUsersForClaimAsync (new Claim ($"Type_{complement}", $"Value_{complement}"), default);

                // assert
                Assert.NotNull (users);
                Assert.NotEmpty (users);
            }
        }
    }
}