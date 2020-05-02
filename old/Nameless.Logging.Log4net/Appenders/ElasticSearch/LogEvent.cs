using System;
using System.Collections.Generic;
using System.Linq;
using log4net.Core;

namespace Nameless.Logging.Log4net.Appenders.ElasticSearch {
    public class LogEvent {
        #region Public Properties

        public string TimeStamp { get; set; }
        public string Message { get; set; }
        public object MessageObject { get; set; }
        public object Exception { get; set; }
        public string LoggerName { get; set; }
        public string Domain { get; set; }
        public string Identity { get; set; }
        public string Level { get; set; }
        public string ClassName { get; set; }
        public string FileName { get; set; }
        public string LineNumber { get; set; }
        public string FullInfo { get; set; }
        public string MethodName { get; set; }
        public string Fix { get; set; }
        public IDictionary<string, string> Properties { get; set; } = new Dictionary<string, string> ();
        public string UserName { get; set; }
        public string ThreadName { get; set; }
        public string HostName { get; set; }

        #endregion

        #region Public Static Methods

        public static IEnumerable<LogEvent> CreateMany (IEnumerable<LoggingEvent> loggingEvents) {
            return loggingEvents.Select (@event => Create (@event)).ToArray ();
        }

        #endregion

        #region Private Static Methods

        private static LogEvent Create (LoggingEvent loggingEvent) {
            var logEvent = new LogEvent {
                LoggerName = loggingEvent.LoggerName,
                Domain = loggingEvent.Domain,
                Identity = loggingEvent.Identity,
                ThreadName = loggingEvent.ThreadName,
                UserName = loggingEvent.UserName,
                TimeStamp = loggingEvent.TimeStamp.ToUniversalTime ().ToString ("O"),
                Exception = loggingEvent.ExceptionObject != null ? ExceptionObject.Create (loggingEvent.ExceptionObject) : new object (),
                Message = loggingEvent.RenderedMessage,
                Fix = loggingEvent.Fix.ToString (),
                HostName = Environment.MachineName,
                Level = loggingEvent.Level == null ? null : loggingEvent.Level.DisplayName
            };

            // Added special handling of the MessageObject since it may be an exception. 
            // Exception Types require specialized serialization to prevent serialization exceptions.
            if (loggingEvent.MessageObject != null) {
                logEvent.MessageObject = loggingEvent.MessageObject is Exception
                    ? ExceptionObject.Create (loggingEvent.MessageObject as Exception)
                    : logEvent.MessageObject = loggingEvent.MessageObject;
            } else { logEvent.MessageObject = new object (); }

            if (loggingEvent.LocationInformation != null) {
                logEvent.ClassName = loggingEvent.LocationInformation.ClassName;
                logEvent.FileName = loggingEvent.LocationInformation.FileName;
                logEvent.LineNumber = loggingEvent.LocationInformation.LineNumber;
                logEvent.FullInfo = loggingEvent.LocationInformation.FullInfo;
                logEvent.MethodName = loggingEvent.LocationInformation.MethodName;
            }

            AddProperties (loggingEvent, logEvent);

            return logEvent;
        }

        private static void AddProperties (LoggingEvent loggingEvent, LogEvent logEvent) {
            var properties = loggingEvent.Properties;
            var keys = properties.GetKeys ();
            keys.Each (key => {
                var value = properties[key];
                logEvent.Properties.Add (key, (value != null ? value.ToString () : string.Empty));
            });
            logEvent.Properties.Add ("@timestamp", loggingEvent.TimeStamp.ToUniversalTime ().ToString ("O"));
        }

        #endregion
    }
}