namespace Models.Enums;

/// <summary>
/// Defines the types of references that can be used in method invocations.
/// </summary>
public enum TypeReference
{
    /// <summary>
    /// Represents a reference to the current class instance.
    /// </summary>
    This,

    /// <summary>
    /// Represents a reference to the base class.
    /// </summary>
    Base,

    /// <summary>
    /// Represents a reference to a class attribute.
    /// </summary>
    Attribute,

    /// <summary>
    /// Represents a reference to a method parameter.
    /// </summary>
    Parameter,

    /// <summary>
    /// Represents a reference to a local variable within a method.
    /// </summary>
    LocalVariable,

    /// <summary>
    /// Represents a reference to a Static method.
    /// </summary>
    Static,

    /// <summary>
    /// Represents a reference to a Static Attribute method.
    /// </summary>
    StaticAttribute,
}
