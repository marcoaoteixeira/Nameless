using Nameless.ObjectModel;
using Nameless.Results;

namespace Nameless.Compression.Responses;

public class DecompressArchiveResponse : Result<DecompressArchiveMetadata> {
    private DecompressArchiveResponse(DecompressArchiveMetadata value, Error[] errors)
        : base(value, errors) { }

    public static implicit operator DecompressArchiveResponse(DecompressArchiveMetadata value) {
        return new DecompressArchiveResponse(value, errors: []);
    }
    
    public static implicit operator DecompressArchiveResponse(Error[] errors) {
        return new DecompressArchiveResponse(value: default, errors);
    }

    public static implicit operator DecompressArchiveResponse(Error error) {
        return new DecompressArchiveResponse(value: default, errors: [error]);
    }
}