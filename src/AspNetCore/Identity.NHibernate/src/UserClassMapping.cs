using Nameless.Persistence.NHibernate;
using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace Nameless.AspNetCore.Identity.NHibernate {
    public sealed class UserClassMapping : ClassMapping<User> {
        #region Public Constructors

        public UserClassMapping () {
            Table ("users");
            
            Id (prop => prop.Id, mapper => {
                mapper.Column ("id");
                mapper.Type (NHibernateUtil.Guid);
                mapper.Generator (Generators.Assigned);
            });

            Property (prop => prop.Email, mapping => {
                mapping.Column ("email");
                mapping.Index ("idx_users_email");
                mapping.Length (512);
                mapping.NotNullable (true);
                mapping.Type (NHibernateUtil.String);
                mapping.Unique (true);
            });

            Property (prop => prop.EmailConfirmed, mapping => {
                mapping.Column (column => {
                    column.Name ("email_confirmed");
                    column.Default (false);
                });
                mapping.NotNullable (true);
                mapping.Type (NHibernateUtil.Boolean);
            });

            Property (prop => prop.NormalizedEmail, mapping => {
                mapping.Column ("normalized_email");
                mapping.Index ("idx_users_normalized_email");
                mapping.Length (512);
                mapping.NotNullable (true);
                mapping.Type (NHibernateUtil.String);
            });

            Property (prop => prop.UserName, mapping => {
                mapping.Column ("user_name");
                mapping.Index ("idx_users_user_name");
                mapping.Length (512);
                mapping.NotNullable (true);
                mapping.Type (NHibernateUtil.String);
            });

            Property (prop => prop.NormalizedUserName, mapping => {
                mapping.Column ("normalized_user_name");
                mapping.Index ("idx_users_normalized_user_name");
                mapping.Length (512);
                mapping.NotNullable (true);
                mapping.Type (NHibernateUtil.String);
            });

            Property (prop => prop.PhoneNumber, mapping => {
                mapping.Column ("phone_number");
                mapping.Length (64);
                mapping.Type (NHibernateUtil.String);
            });

            Property (prop => prop.PhoneNumberConfirmed, mapping => {
                mapping.Column (column => {
                    column.Name ("phone_number_confirmed");
                    column.Default (false);
                });
                mapping.NotNullable (true);
                mapping.Type (NHibernateUtil.Boolean);
            });

            Property (prop => prop.LockoutEnabled, mapping => {
                mapping.Column (column => {
                    column.Name ("lockout_enabled");
                    column.Default (false);
                });
                mapping.NotNullable (true);
                mapping.Type (NHibernateUtil.Boolean);
            });

            Property (prop => prop.LockoutEnd, mapping => {
                mapping.Column ("lockout_end");
                mapping.NotNullable (false);
                mapping.Type<DateTimeOffsetUserType> ();
            });

            Property (prop => prop.AccessFailedCount, mapping => {
                mapping.Column (column => {
                    column.Name ("access_failed_count");
                    column.Default (0);
                });
                mapping.NotNullable (true);
                mapping.Type (NHibernateUtil.Int32);
            });

            Property (prop => prop.PasswordHash, mapping => {
                mapping.Column ("password_hash");
                mapping.Length (512);
                mapping.NotNullable (false);
                mapping.Type (NHibernateUtil.String);
            });

            Property (prop => prop.SecurityStamp, mapping => {
                mapping.Column ("security_stamp");
                mapping.Length (512);
                mapping.NotNullable (true);
                mapping.Type (NHibernateUtil.String);
            });

            Property (prop => prop.TwoFactorEnabled, mapping => {
                mapping.Column (column => {
                    column.Name ("two_factor_enabled");
                    column.Default (false);
                });
                mapping.NotNullable (true);
                mapping.Type (NHibernateUtil.Boolean);
            });

            Property (prop => prop.AvatarUrl, mapping => {
                mapping.Column ("avatar_url");
                mapping.Length (2048);
                mapping.NotNullable (false);
                mapping.Type (NHibernateUtil.String);
            });
        }

        #endregion
    }
}