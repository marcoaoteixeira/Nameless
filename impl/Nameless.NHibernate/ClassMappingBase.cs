using NHibernate.Mapping.ByCode.Conformist;

namespace Nameless.NHibernate {
    public abstract class ClassMappingBase<T> : ClassMapping<T> where T : class {
        #region Protected Constructors

        protected ClassMappingBase (string tableName, ClassMappingOptions options = null) {
            Prevent.ParameterNullOrWhiteSpace (tableName, nameof (tableName));
            
            var currentOptions = options ?? new ClassMappingOptions ();

            if (!string.IsNullOrWhiteSpace (currentOptions.DbSchemaName)) {
                Schema (currentOptions.DbSchemaName);
            }

            var currentTableName = !string.IsNullOrWhiteSpace (currentOptions.TablePrefix) ? $"{currentOptions.TablePrefix}_{tableName}" : tableName;
            Table (currentTableName);
        }

        #endregion  
    }
}