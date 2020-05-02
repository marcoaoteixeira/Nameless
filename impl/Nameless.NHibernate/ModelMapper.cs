using System;
using System.Collections.Generic;

namespace Nameless.NHibernate {
    public sealed class ModelMapper : global::NHibernate.Mapping.ByCode.ModelMapper {
        #region Private Properties

        private ClassMappingOptions Options { get; set; }

        #endregion

        #region Public Constructors

        public ModelMapper (ClassMappingOptions options = null) { SetOptions (options); }

        public ModelMapper (global::NHibernate.Mapping.ByCode.IModelInspector modelInspector, ClassMappingOptions options = null) : base (modelInspector) { SetOptions (options); }

        public ModelMapper (global::NHibernate.Mapping.ByCode.IModelInspector modelInspector, global::NHibernate.Mapping.ByCode.Impl.ICandidatePersistentMembersProvider membersProvider, ClassMappingOptions options = null) : base (modelInspector, membersProvider) { SetOptions (options); }

        public ModelMapper (global::NHibernate.Mapping.ByCode.IModelInspector modelInspector, global::NHibernate.Mapping.ByCode.IModelExplicitDeclarationsHolder explicitDeclarationsHolder, ClassMappingOptions options = null) : base (modelInspector, explicitDeclarationsHolder) { SetOptions (options); }

        public ModelMapper (global::NHibernate.Mapping.ByCode.IModelInspector modelInspector, global::NHibernate.Mapping.ByCode.IModelExplicitDeclarationsHolder explicitDeclarationsHolder, global::NHibernate.Mapping.ByCode.Impl.ICustomizersHolder customizerHolder, global::NHibernate.Mapping.ByCode.Impl.ICandidatePersistentMembersProvider membersProvider, ClassMappingOptions options = null) : base (modelInspector, explicitDeclarationsHolder, customizerHolder, membersProvider) { SetOptions (options); }

        #endregion

        #region Private Methods

        private void SetOptions (ClassMappingOptions options) {
            Options = options ?? new ClassMappingOptions ();
        }

        #endregion

        #region Public New Methods

        public new void AddMapping (Type type) {
            var provider = type.IsAssignableToGenericType (typeof (ClassMappingBase<>)) ?
                (global::NHibernate.Mapping.ByCode.IConformistHoldersProvider) Activator.CreateInstance (type, args : new object[] { Options }) :
                (global::NHibernate.Mapping.ByCode.IConformistHoldersProvider) Activator.CreateInstance (type);
            AddMapping (provider);
        }

        public new void AddMapping<T> () where T : global::NHibernate.Mapping.ByCode.IConformistHoldersProvider, new () {
            AddMapping (typeof (T));
        }

        public new void AddMappings (IEnumerable<Type> types) {
            foreach (var type in types) {
                if (typeof (global::NHibernate.Mapping.ByCode.IConformistHoldersProvider).IsAssignableFrom (type) && !type.IsGenericTypeDefinition) {
                    AddMapping (type);
                }
            }
        }

        #endregion
    }
}