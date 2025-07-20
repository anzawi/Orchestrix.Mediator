namespace Orchestrix.Mediator.Core.Execution.Interfaces;

/// <summary>
/// Represents a service responsible for handling the invocation of a request handler,
/// providing a mechanism to execute pre-configured request handlers with the required dependencies.
/// </summary>
/// <typeparam name="TResponse">The type of the response returned by the handler.</typeparam>
internal interface IHandlerInvoker<TResponse>
{
    /// <summary>
    /// Invokes the handler executor for a specified request and returns the response.
    /// </summary>
    /// <param name="provider">
    /// The IServiceProvider instance used to resolve service dependencies required for the handler.
    /// </param>
    /// <param name="request">
    /// The object representing the request to be processed by the handler.
    /// </param>
    /// <param name="ct">
    /// A CancellationToken to observe while the asynchronous operation is performed.
    /// </param>
    /// <returns>
    /// A ValueTask of type TResponse that represents the asynchronous operation result returned by the handler.
    /// </returns>
    ValueTask<TResponse> Invoke(IServiceProvider provider, object request, CancellationToken ct);
}