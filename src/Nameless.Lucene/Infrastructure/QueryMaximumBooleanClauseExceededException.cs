namespace Nameless.Lucene.Infrastructure;

public class QueryMaximumBooleanClauseExceededException : Exception {
    public QueryMaximumBooleanClauseExceededException()
        : base(message: "The number of clauses in the boolean query exceeds the maximum allowed (1024).") {  }

    public QueryMaximumBooleanClauseExceededException(string message)
        : base(message) { }
}