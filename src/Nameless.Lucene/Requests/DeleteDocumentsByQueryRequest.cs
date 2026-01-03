using Lucene.Net.Search;

namespace Nameless.Lucene.Requests;

public sealed record DeleteDocumentsByQueryRequest(Query Query);