using System.ComponentModel;

namespace Nameless.Test.Utils {
    /// <summary>
    /// Attribute to specify that the current test method should only run on
    /// the developer machine. Due to resources that may not be available on
    /// the build machine.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class RunsOnDevMachineAttribute : CategoryAttribute {
        public RunsOnDevMachineAttribute()
            : base("RunsOnDevMachine") { }
    }
}
