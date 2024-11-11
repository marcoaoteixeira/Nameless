namespace Nameless;

[Serializable]
public class PathResolutionException : Exception {
    public PathResolutionException() { }
    public PathResolutionException(string message) : base(message) { }
    public PathResolutionException(string message, Exception inner) : base(message, inner) { }
}