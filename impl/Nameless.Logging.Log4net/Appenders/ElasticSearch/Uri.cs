using System;
using System.Collections.Specialized;
using System.Data.Common;
using SystemUri = System.Uri;

namespace Nameless.Logging.Log4net.Appenders.ElasticSearch {
    public class Uri {
        #region Private Constants

        private const string SCHEMA_KEY = "Scheme";
        private const string USER_KEY = "User";
        private const string PASSWORD_KEY = "Pwd";
        private const string SERVER_KEY = "Server";
        private const string PORT_KEY = "Port";
        private const string INDEX_KEY = "Index";
        private const string ROLLING_KEY = "Rolling";
        private const string BUFFER_SIZE_KEY = "BufferSize";
        private const string ROUTING_KEY = "Routing";

        #endregion

        #region Private Read-Only Fields

        private readonly StringDictionary _parts;

        #endregion

        #region Public Properties

        public string User {
            get { return _parts[USER_KEY]; }
        }

        public string Password {
            get { return _parts[PASSWORD_KEY]; }
        }

        public string Scheme {
            get { return _parts[SCHEMA_KEY] ?? "http"; }
        }

        public string Server {
            get { return _parts[SERVER_KEY]; }
        }

        public string Port {
            get { return _parts[PORT_KEY]; }

        }

        public string Routing {
            get {
                var routing = _parts[ROUTING_KEY];
                return !string.IsNullOrWhiteSpace (routing)
                    ? string.Format ("?routing={0}", routing)
                    : string.Empty;
            }
        }

        public string Bulk {
            get {
                var bufferSize = _parts[BUFFER_SIZE_KEY];
                return (Convert.ToInt32 (bufferSize) > 1)
                    ? "/_bulk"
                    : string.Empty;
            }
        }

        public string Index {
            get {
                var index = _parts[INDEX_KEY];
                return IsRollingIndex (_parts)
                    ? $"{index}-{Clock.Date.ToString ("yyyy.MM.dd")}"
                    : index;
            }
        }

        #endregion

        #region Public Constructors

        public Uri (StringDictionary parts) {
            Prevent.ParameterNull (parts, nameof (parts));

            this._parts = parts;
        }

        #endregion

        #region Public Static Implicit Operators

        public static implicit operator SystemUri (Uri uri) {
            if (!string.IsNullOrWhiteSpace (uri.User) && !string.IsNullOrWhiteSpace (uri.Password)) {
                return new SystemUri ($"{uri.Scheme}://{uri.User}:{uri.Password}@{uri.Server}:{uri.Port}/{uri.Index}/logEvent{uri.Routing}{uri.Bulk}");
            }

            return string.IsNullOrEmpty (uri.Port)
                ? new SystemUri ($"{uri.Scheme}://{uri.Server}/{uri.Index}/logEvent{uri.Routing}{uri.Bulk}")
                : new SystemUri ($"{uri.Scheme}://{uri.Server}:{uri.Port}/{uri.Index}/logEvent{uri.Routing}{uri.Bulk}");
        }

        #endregion

        #region Public Static Methods

        public static Uri For (string connectionString) {
            Prevent.ParameterNullOrWhiteSpace (connectionString, nameof (connectionString));

            var builder = new DbConnectionStringBuilder {
                ConnectionString = connectionString.Replace ("{", "\"").Replace ("}", "\"")
            };
            var parts = new StringDictionary ();
            foreach (string key in builder.Keys) {
                parts[key] = Convert.ToString (builder[key]);
            }
            return new Uri (parts);
        }

        #endregion

        #region Private Static Methods

        private static bool IsRollingIndex (StringDictionary parts) {
            return parts.ContainsKey (ROLLING_KEY)
                && bool.TryParse (parts[ROLLING_KEY], out bool dummy)
                && dummy;
        }

        #endregion
    }
}