using System;
using System.Collections.Generic;

namespace Nameless.Logging.Log4net.Appenders.ElasticSearch {
    public class Repository : IRepository {
        #region Private Read-Only Fields

        readonly IHttpClient _httpClient;
        readonly Uri _uri;

        #endregion

        #region Public Constructors

        Repository (IHttpClient httpClient, Uri uri) {
            Prevent.ParameterNull (httpClient, nameof (httpClient));
            Prevent.ParameterNull (uri, nameof (uri));

            _httpClient = httpClient;
            _uri = uri;
        }

        #endregion

        #region Public Static Methods

        public static IRepository Create (string connectionString) {
            return Create (new HttpClient (), connectionString);
        }

        public static IRepository Create (IHttpClient httpClient, string connectionString) {
            return new Repository (httpClient, Uri.For (connectionString));
        }

        #endregion

        #region IRepository Members

        /// <summary>
        /// Post the event(s) to the Elasticsearch API. If the bufferSize in the connection
        /// string is set to more than 1, assume we use the _bulk API for better speed and
        /// efficiency
        /// </summary>
        /// <param name="logEvents">A collection of logEvents</param>
        /// <param name="bufferSize">The BufferSize as set in the connection string details</param>
        public void Add (IEnumerable<LogEvent> logEvents, int bufferSize) {
            try {
                if (bufferSize <= 1) {
                    // Post the logEvents one at a time throught the ES insert API
                    logEvents.Each (evt => _httpClient.Post (_uri, evt));
                } else {
                    // Post the logEvents all at once using the ES _bulk API
                    _httpClient.PostBulk (_uri, logEvents);
                }
            } catch { /* swallow exception */ }
        }

        #endregion

    }
}