using System;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace Nameless.Skeleton.Web {
    internal static class ControllerHelper {
        #region Private Constants

        private const string CONTROLLER_SUFFIX = "Controller";

        #endregion

        #region Internal Static Methods

        internal static string GetControllerName (Type controllerType) {
            if (controllerType == null) { return null; }

            // As stupid as that
            var attr = controllerType.GetCustomAttribute<ControllerAttribute> ();

            return attr != null ? controllerType.Name.TrimSuffix (CONTROLLER_SUFFIX) : null;
        }

        internal static string GetControllerName (ControllerBase controller) {
            if (controller == null) { return null; }

            // As stupid as that
            return controller.GetType ().Name.TrimSuffix (CONTROLLER_SUFFIX);
        }

        #endregion
    }
}