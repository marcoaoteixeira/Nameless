using System;
using System.Collections;
using System.Collections.Generic;

namespace Nameless.Environment {
    public sealed class HostingEnvironment : IHostingEnvironment {
        #region Private Read-Only Fields

        private readonly Microsoft.Extensions.Hosting.IHostEnvironment _hostEnvironment;

        #endregion

        #region Public Constructors

        public HostingEnvironment (Microsoft.Extensions.Hosting.IHostEnvironment hostEnvironment) {
            Prevent.ParameterNull (hostEnvironment, nameof (hostEnvironment));

            _hostEnvironment = hostEnvironment;
        }

        #endregion

        #region Private Static Methods

        private static EnvironmentVariableTarget Parse (VariableTarget target) {
            switch (target) {
                case VariableTarget.Process:
                    return EnvironmentVariableTarget.Process;
                case VariableTarget.User:
                    return EnvironmentVariableTarget.User;
                case VariableTarget.Machine:
                    break;
                default:
                    return EnvironmentVariableTarget.Machine;
            }

            return EnvironmentVariableTarget.Machine;
        }

        #endregion

        #region IHostingEnvironment Members

        public string EnvironmentName => _hostEnvironment.EnvironmentName;

        public string ApplicationName => _hostEnvironment.ApplicationName;

        public string ApplicationBasePath => _hostEnvironment.ContentRootPath;

        public object GetData (string key) => AppDomain.CurrentDomain.GetData (key);

        public void SetData (string key, object data) => AppDomain.CurrentDomain.SetData (key, data);

        public string GetVariable (string key, VariableTarget target = VariableTarget.User) {
            return System.Environment.GetEnvironmentVariable (key, Parse (target));
        }

        public IDictionary<string, string> GetVariables (VariableTarget target) {
            var variables = System.Environment.GetEnvironmentVariables (Parse (target));
            var result = new Dictionary<string, string> ();
            foreach (DictionaryEntry kvp in variables) {
                result.Add ((string) kvp.Key, (string) kvp.Value);
            }
            return result;
        }

        public void SetVariable (string key, string variable, VariableTarget target = VariableTarget.User) {
            System.Environment.SetEnvironmentVariable (key, variable, Parse (target));
        }

        #endregion
    }
}