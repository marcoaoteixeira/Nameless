using System;
using System.Collections;
using System.Collections.Generic;
using MS_Environment = System.Environment;
using MS_HostEnvironment = Microsoft.Extensions.Hosting.IHostEnvironment;

namespace Nameless.Environment {
    public sealed class HostingEnvironment : IHostingEnvironment {
        #region Private Read-Only Fields

        private readonly MS_HostEnvironment _hostingEnvironment;

        #endregion

        #region Public Constructors

        public HostingEnvironment (MS_HostEnvironment hostingEnvironment) {
            Prevent.ParameterNull (hostingEnvironment, nameof (hostingEnvironment));

            _hostingEnvironment = hostingEnvironment;
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

        public string ApplicationBasePath => _hostingEnvironment.ContentRootPath;

        public object GetData (string key) => AppDomain.CurrentDomain.GetData (key);

        public void SetData (string key, object data) => AppDomain.CurrentDomain.SetData (key, data);

        public string GetVariable (string key, VariableTarget target = VariableTarget.User) {
            return MS_Environment.GetEnvironmentVariable (key, Parse (target));
        }

        public IDictionary<string, string> GetVariables (VariableTarget target) {
            var variables = MS_Environment.GetEnvironmentVariables (Parse (target));
            var result = new Dictionary<string, string> ();
            foreach (DictionaryEntry kvp in variables) {
                result.Add ((string) kvp.Key, (string) kvp.Value);
            }
            return result;
        }

        public void SetVariable (string key, string variable, VariableTarget target = VariableTarget.User) {
            MS_Environment.SetEnvironmentVariable (key, variable, Parse (target));
        }

        #endregion
    }
}