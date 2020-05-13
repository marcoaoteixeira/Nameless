namespace Nameless.PubSub {
    public sealed class Message {
        #region Public Properties

        public string Type { get; set; }
        public object Payload { get; set; }

        #endregion
    }
}