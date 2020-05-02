using System;

namespace Nameless.IoC.Autofac {

    /// <summary>
    /// Attribute used to wire services via property injection.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class PropertyAutoWireAttribute : Attribute {
    }
}