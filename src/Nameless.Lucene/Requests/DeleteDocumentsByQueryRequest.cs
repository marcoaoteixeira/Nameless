using Lucene.Net.Search;

namespace Nameless.Lucene.Requests;

public record DeleteDocumentsByQueryRequest(Query Query);