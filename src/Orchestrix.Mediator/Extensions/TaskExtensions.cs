namespace Orchestrix.Mediator.Extensions;

/// <summary>
/// Provides extension methods for <see cref="Task"/> to enhance task handling capabilities.
/// </summary>
internal static class TaskExtensions
{
    /// <summary>
    /// Wraps a Task of type T into a ValueTask of the same type.
    /// </summary>
    /// <typeparam name="T">The type of the task result.</typeparam>
    /// <param name="task">The Task instance to be wrapped.</param>
    /// <returns>A ValueTask that represents the same operation as the input Task.</returns>
    public static ValueTask<T> WrapAsValueTask<T>(this Task<T> task) => new(task);
}