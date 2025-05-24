using Transformers.Abstractions;

namespace IBusinessLogic;
public interface ITransformerService
{
    /// <summary>
    /// Carga todos los transformadores disponibles.
    /// </summary>
    void LoadTransformers();

    /// <summary>
    /// Obtiene información de todos los transformadores disponibles.
    /// </summary>
    /// <returns>Lista de información de transformadores.</returns>
    IEnumerable<TransformerInfo> GetAvailableTransformers();

    /// <summary>
    /// Transforma un resultado de ejecución utilizando el transformador especificado.
    /// </summary>
    /// <param name="executionResult">Resultado original de la ejecución.</param>
    /// <param name="transformerId">ID del transformador (opcional).</param>
    /// <returns>Respuesta transformada.</returns>
    TransformedResponse TransformExecution(string executionResult, string transformerId = null);

    /// <summary>
    /// Obtiene un transformador por su identificador.
    /// </summary>
    /// <param name="id">ID del transformador.</param>
    /// <returns>El transformador si existe, null si no.</returns>
    IResponseTransformer GetTransformerById(string id);
}
