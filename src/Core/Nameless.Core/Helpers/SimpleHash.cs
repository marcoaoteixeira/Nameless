namespace Nameless.Helpers {
    public static class SimpleHash {
        #region Public Static Methods

        public static int Compute(params object?[] args) {
            var hash = 13;
            unchecked {
                foreach (var arg in args) {
                    hash += (arg != null ? arg.GetHashCode() : 0) * 7;
                }
            }
            return hash;
        }

        #endregion
    }
}
