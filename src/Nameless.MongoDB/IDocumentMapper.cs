using MongoDB.Bson.Serialization;

namespace Nameless.MongoDB;

/// <summary>
/// Interface for mapping documents to BsonClassMap.
/// </summary>
public interface IDocumentMapper {
    /// <summary>
    /// Creates a BsonClassMap for the document type.
    /// </summary>
    /// <returns>
    /// A BsonClassMap that maps the document type to BSON serialization.
    /// </returns>
    BsonClassMap CreateMap();
}