namespace Transformers.Abstractions;

/// <summary>
/// Define un transformador de respuestas para la ejecución de métodos.
/// Los transformadores convierten los resultados de ejecución a diferentes formatos
/// para su visualización (texto plano, HTML, imágenes, etc.)
/// </summary>
public interface IResponseTransformer
{
    /// <summary>
    /// Gets identificador único del transformador.
    /// </summary>
    string Id { get; }

    /// <summary>
    /// Gets nombre descriptivo para mostrar en la UI.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets orden de visualización en el menú (menor número = mayor prioridad).
    /// </summary>
    int DisplayOrder { get; }

    /// <summary>
    /// Gets tipo MIME del contenido generado (ej: "text/plain", "text/html", "image/png").
    /// </summary>
    string ContentType { get; }

    /// <summary>
    /// Transforma un resultado de ejecución al formato deseado.
    /// </summary>
    /// <param name="executionResult">Texto original del resultado.</param>
    /// <returns>Texto transformado.</returns>
    string Transform(string executionResult);
}
