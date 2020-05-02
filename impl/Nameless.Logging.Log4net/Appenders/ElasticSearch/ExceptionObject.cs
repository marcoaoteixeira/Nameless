using System;
using System.Collections;

namespace Nameless.Logging.Log4net.Appenders.ElasticSearch {
    public class ExceptionObject {
        #region Public Properties

        public string Type { get; set; }
        public string Message { get; set; }
        public string HelpLink { get; set; }
        public string Source { get; set; }
        public int HResult { get; set; }
        public string StackTrace { get; set; }
        public IDictionary Data { get; set; }
        public ExceptionObject InnerException { get; set; }

        #endregion

        #region Public Static Methods

        public static ExceptionObject Create (Exception ex) {
            if (ex == null) { return null; }
            var serializable = Parse (ex);
            FetchInnerException (serializable, ex.InnerException, maxDeep: 3);
            return serializable;
        }

        #endregion

        #region Private Static Methods

        private static ExceptionObject Parse (Exception ex) {
            return new ExceptionObject {
                Type = ex.GetType ().FullName,
                Message = ex.Message,
                HelpLink = ex.HelpLink,
                Source = ex.Source,
                HResult = ex.HResult,
                StackTrace = ex.StackTrace,
                Data = ex.Data
            };
        }

        private static void FetchInnerException (ExceptionObject json, Exception innerException, int currentDeep = 0, int maxDeep = 5) {
            if (innerException == null || (currentDeep == maxDeep)) { return; }
            json.InnerException = Parse (innerException);
            FetchInnerException (json.InnerException, innerException.InnerException, currentDeep, maxDeep);
        }

        #endregion
    }
}