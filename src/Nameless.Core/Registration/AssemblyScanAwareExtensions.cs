namespace Nameless.Registration;

public static class AssemblyScanAwareExtensions {
    extension<TSelf>(IAssemblyScanAware<TSelf> self)
        where TSelf : IAssemblyScanAware<TSelf> {
        public IEnumerable<Type> GetImplementationsFor<TType>() {
            return self.GetImplementationsFor<TType>(includeGenericDefinition: false);
        }

        public IEnumerable<Type> GetImplementationsFor(Type type) {
            return self.GetImplementationsFor(type, includeGenericDefinition: false);
        }
    }
}