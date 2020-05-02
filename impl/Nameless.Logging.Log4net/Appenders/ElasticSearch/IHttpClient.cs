using System.Collections.Generic;

namespace Nameless.Logging.Log4net.Appenders.ElasticSearch {
    public interface IHttpClient {
        #region Methods

        void Post (Uri uri, LogEvent logEvent);
        void PostBulk (Uri uri, IEnumerable<LogEvent> logEvents);

        #endregion
    }
}