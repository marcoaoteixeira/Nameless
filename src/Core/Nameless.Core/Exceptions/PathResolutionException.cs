namespace Nameless {
    [Serializable]
    public class PathResolutionException : Exception {
        #region Public Constructors

        public PathResolutionException() { }
        public PathResolutionException(string message) : base(message) { }
        public PathResolutionException(string message, Exception inner) : base(message, inner) { }

        #endregion
    }
}