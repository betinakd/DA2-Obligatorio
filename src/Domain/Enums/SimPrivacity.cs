namespace Domain.Enums;

/// <summary>
/// Represents the state of a simulation.
/// </summary>
public enum SimPrivacity
{
    /// <summary>
    /// Represents a normal simulation state.
    /// </summary>
    Private,

    /// <summary>
    /// Represents an abstract simulation state.
    /// </summary>
    Protected,

    /// <summary>
    /// Represents a sealed simulation state.
    /// </summary>
    Public,
}
