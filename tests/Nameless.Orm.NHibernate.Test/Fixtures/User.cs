namespace Nameless.Orm.NHibernate.Test.Fixtures {
    public class User : EntityBase {
        public virtual string Name { get; set; }
        public virtual string Email { get; set; }
        public virtual string Password { get; set; }
        public virtual string Avatar { get; set; }
    }
}
