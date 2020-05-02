namespace Nameless.Orm.NHibernate.Test.Fixtures {
    public class UserClassMapping : EntityClassMappingBase<User> {
        public UserClassMapping ()
            : base ("users", "id") {

            Property (_ => _.Name, mapping => mapping.Column ("name"));
            Property (_ => _.Email, mapping => mapping.Column ("email"));
            Property (_ => _.Password, mapping => mapping.Column ("password"));
            Property (_ => _.Avatar, mapping => mapping.Column ("avatar"));
        }
    }
}
