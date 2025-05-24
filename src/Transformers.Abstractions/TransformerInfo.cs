namespace Transformers.Abstractions;

/// <summary>
/// Información básica sobre un transformador disponible.
/// </summary>
public class TransformerInfo
{
    /// <summary>
    /// Gets or sets identificador único del transformador.
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// Gets or sets nombre para mostrar en la UI.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets tipo MIME del contenido.
    /// </summary>
    public string? ContentType { get; set; }
}
