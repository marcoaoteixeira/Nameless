using System.Collections.Generic;

namespace Nameless.Logging.Log4net.Appenders.ElasticSearch {
    public interface IRepository {
        #region Methods

        void Add (IEnumerable<LogEvent> logEvents, int bufferSize);

        #endregion
    }
}