using System.Collections;
using Sys_Environment = System.Environment;

namespace Nameless.Environment.System {
    public sealed class HostEnvironment : IHostEnvironment {
        #region Private Read-Only Fields

        private readonly string _environmentName;
        private readonly string _applicationName;
        private readonly string _applicationBasePath;

        #endregion

        #region Public Constructors

        public HostEnvironment(string environmentName, string applicationName, string applicationBasePath) {
            Prevent.Against.NullOrWhiteSpace(environmentName, nameof(environmentName));
            Prevent.Against.NullOrWhiteSpace(applicationName, nameof(applicationName));
            Prevent.Against.NullOrWhiteSpace(applicationBasePath, nameof(applicationBasePath));

            _environmentName = environmentName;
            _applicationName = applicationName;
            _applicationBasePath = applicationBasePath;
        }

        #endregion

        #region Private Static Methods

        private static EnvironmentVariableTarget Translate(VariableTarget target) {
            return target switch {
                VariableTarget.Process => EnvironmentVariableTarget.Process,
                VariableTarget.User => EnvironmentVariableTarget.User,
                VariableTarget.Machine => EnvironmentVariableTarget.Machine,
                _ => EnvironmentVariableTarget.User,
            };
        }

        #endregion

        #region IHostingEnvironment Members

        public string EnvironmentName => _environmentName;

        public string ApplicationName => _applicationName;

        public string ApplicationBasePath => _applicationBasePath;

        public object? GetData(string key) => AppDomain.CurrentDomain.GetData(key);

        public void SetData(string key, object data) => AppDomain.CurrentDomain.SetData(key, data);

        public string? GetVariable(string key, VariableTarget target = VariableTarget.User) {
            return Sys_Environment.GetEnvironmentVariable(key, Translate(target));
        }

        public IDictionary<string, string?> GetVariables(VariableTarget target) {
            var variables = Sys_Environment.GetEnvironmentVariables(Translate(target));
            var result = new Dictionary<string, string?>();
            if (variables != null) {
                foreach (DictionaryEntry kvp in variables) {
                    result.Add((string)kvp.Key, kvp.Value as string);
                }
            }
            return result;
        }

        public void SetVariable(string key, string variable, VariableTarget target = VariableTarget.User) {
            Sys_Environment.SetEnvironmentVariable(key, variable, Translate(target));
        }

        #endregion
    }
}