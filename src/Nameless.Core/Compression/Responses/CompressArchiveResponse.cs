using Nameless.ObjectModel;
using Nameless.Results;

namespace Nameless.Compression.Responses;

public class CompressArchiveResponse : Result<CompressArchiveMetadata> {
    private CompressArchiveResponse(CompressArchiveMetadata value, Error[] errors)
        : base(value, errors) {
    }

    public static implicit operator CompressArchiveResponse(CompressArchiveMetadata value) {
        return new CompressArchiveResponse(value, errors: []);
    }

    public static implicit operator CompressArchiveResponse(Error[] errors) {
        return new CompressArchiveResponse(value: default, errors);
    }

    public static implicit operator CompressArchiveResponse(Error error) {
        return new CompressArchiveResponse(value: default, errors: [error]);
    }
}