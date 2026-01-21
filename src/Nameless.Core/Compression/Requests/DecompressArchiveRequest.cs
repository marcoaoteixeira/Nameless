namespace Nameless.Compression.Requests;

public class DecompressArchiveRequest {
    public required string SourceFilePath { get; set; }
    
    public required string DestinationDirectoryPath { get; set; }
}