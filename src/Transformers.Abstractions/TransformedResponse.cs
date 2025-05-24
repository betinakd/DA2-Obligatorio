namespace Transformers.Abstractions;

public class TransformedResponse
{
    public string? OriginalResult { get; set; }
    public string? TransformedResult { get; set; }
    public string? ContentType { get; set; }
    public string? TransformerId { get; set; }
    public List<TransformerInfo> AvailableTransformers { get; set; } = [];
}
