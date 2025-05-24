namespace Transformers.Abstractions;

/// <summary>
/// Representa el resultado de una ejecución transformada.
/// </summary>
public class TransformedResponse
{
    /// <summary>
    /// Gets or sets resultado original de la ejecución.
    /// </summary>
    public string? OriginalResult { get; set; }

    /// <summary>
    /// Gets or sets resultado transformado.
    /// </summary>
    public string? TransformedResult { get; set; }

    /// <summary>
    /// Gets or sets tipo MIME del contenido transformado.
    /// </summary>
    public string? ContentType { get; set; }

    /// <summary>
    /// Gets or sets iD del transformador utilizado.
    /// </summary>
    public string? TransformerId { get; set; }

    /// <summary>
    /// Gets or sets lista de transformadores disponibles.
    /// </summary>
    public List<TransformerInfo> AvailableTransformers { get; set; } = [];
}
