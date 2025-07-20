// ReSharper disable once CheckNamespace
namespace Orchestrix.Mediator;

/// <summary>
/// Represents a unit type used primarily for signaling and return types
/// in cases where no meaningful value or result is required.
/// </summary>
/// <remarks>
/// The <see cref="Unit"/> struct is often used in scenarios where the return
/// of a method or function is necessary for type validation, but no actual
/// data needs to be returned. It functions similarly to void but allows
/// for type compatibility.
/// </remarks>
/// <threadsafety>
/// This struct is immutable and thread-safe.
/// </threadsafety>
public readonly struct Unit
{
    /// <summary>
    /// Represents a singleton instance of the <see cref="Unit"/> structure.
    /// </summary>
    /// <remarks>
    /// The <c>Value</c> field is used to represent the default or single instance of the <see cref="Unit"/> structure,
    /// which is often utilized in contexts where a method or operation does not return a meaningful value.
    /// </remarks>
    public static readonly Unit Value = new();

    /// <summary>
    /// Returns a string representation of the Unit struct.
    /// </summary>
    /// <returns>
    /// A string literal "()" representing the Unit struct.
    /// </returns>
    public override string ToString() => "()";
}

/// <summary>
/// Represents a marker struct used to signify a void-like response in the context of Orchestrix.Mediator pipelines.
/// </summary>
/// <remarks>
/// This struct is utilized as a placeholder for operations or request handlers that do not return a meaningful value.
/// It is commonly used in scenarios where the absence of a result needs to be explicitly represented.
/// </remarks>
public readonly struct VoidMarker;