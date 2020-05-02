using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using log4net.Appender;
using log4net.Core;

namespace Nameless.Logging.Log4net.Appenders.ElasticSearch {
    public class ElasticSearchAppender : BufferingAppenderSkeleton {
        #region Private Constants

        private const int DEFAULT_ON_CLOSE_TIMEOUT = 30000;

        #endregion

        #region Private Static Read-Only Fields

        private static readonly string AppenderTypeName = typeof (ElasticSearchAppender).Name;

        #endregion

        #region Private Read-Only Fields

        private readonly ManualResetEvent _manualResetEvent;

        #endregion

        #region Private Fields

        private int _queuedCallbackCount;
        private IRepository _repository;

        #endregion

        #region Public Properties

        public string ConnectionString { get; set; }
        public int OnCloseTimeout { get; set; } = DEFAULT_ON_CLOSE_TIMEOUT;

        #endregion

        #region Public Constructors

        public ElasticSearchAppender () {
            _manualResetEvent = new ManualResetEvent (initialState: true);
        }

        #endregion

        #region Public Override Methods

        public override void ActivateOptions () {
            base.ActivateOptions ();

            ServicePointManager.Expect100Continue = false;

            if (string.IsNullOrWhiteSpace (ConnectionString)) {
                HandleError ("Failed to validate ConnectionString in ActivateOptions. ConnectionString can not be null, empty or white spaces.");
                return;
            }

            // Artificially add the buffer size to the connection string so it can be parsed
            // later to decide if we should send a _bulk API call
            ConnectionString += $";BufferSize={BufferSize}";
            _repository = CreateRepository (ConnectionString);
        }

        #endregion

        #region Protected Virtual Methods

        protected virtual IRepository CreateRepository (string connectionString) {
            return Repository.Create (connectionString);
        }

        protected virtual bool TryAsyncSend (IEnumerable<LoggingEvent> events) {
            return ThreadPool.QueueUserWorkItem (SendBufferCallback, LogEvent.CreateMany (events));
        }

        protected virtual bool TryWaitAsyncSendFinish () {
            return _manualResetEvent.WaitOne (OnCloseTimeout, false);
        }

        #endregion

        #region Protected Override Methods

        protected override void SendBuffer (LoggingEvent[] events) {
            BeginAsyncSend ();
            if (TryAsyncSend (events)) return;
            EndAsyncSend ();
            HandleError ("Failed to async send logging events in SendBuffer");
        }

        protected override void OnClose () {
            base.OnClose ();

            if (TryWaitAsyncSendFinish ()) return;
            HandleError ("Failed to send all queued events in OnClose");
        }

        #endregion


        #region Private Methods

        private void BeginAsyncSend () {
            _manualResetEvent.Reset ();
            Interlocked.Increment (ref _queuedCallbackCount);
        }

        private void SendBufferCallback (object state) {
            try { _repository.Add ((IEnumerable<LogEvent>)state, BufferSize); } catch (Exception ex) { HandleError ($"Failed to addd logEvents to {_repository.GetType ().Name} in SendBufferCallback", ex); } finally { EndAsyncSend (); }
        }

        private void EndAsyncSend () {
            if (Interlocked.Decrement (ref _queuedCallbackCount) > 0) {
                return;
            }
            _manualResetEvent.Set ();
        }

        private void HandleError (string message) {
            ErrorHandler.Error ($"{AppenderTypeName} [{Name}]: {message}.");
        }

        private void HandleError (string message, Exception ex) {
            ErrorHandler.Error ($"{AppenderTypeName} [{Name}]: {message}.", ex, ErrorCode.GenericFailure);
        }

        #endregion
    }
}