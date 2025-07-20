using System.Collections.Concurrent;
using System.Reflection;
using Orchestrix.Mediator.Core.Execution.Interfaces;
using Orchestrix.Mediator.Extensions;

namespace Orchestrix.Mediator.Core.Execution;

/// <inheritdoc />
internal sealed class Mediator(
    IServiceProvider provider,
    IHookExecutor hookExecutor
) : IMediator
{
    /// <summary>
    /// A thread-safe cache that stores invoker instances for handling requests in the mediator.
    /// This cache maps request types to handler invokers, ensuring efficient retrieval and reuse
    /// of handler invoker objects during the processing of commands or queries.
    /// </summary>
    private static readonly ConcurrentDictionary<Type, object> SendCache = new();

    /// <summary>
    /// A thread-safe, static cache used to store handler invoker instances associated with specific request types,
    /// designed to optimize the resolution and invocation of request handlers handling non-returning (void) requests.
    /// </summary>
    /// <remarks>
    /// The cache maps request types to instances of <see cref="IHandlerInvoker{TResponse}" />, where TResponse is
    /// a marker indicating a void result. This allows efficient reuse of handler invoker instances without
    /// re-creating or reflecting on them repeatedly at runtime.
    /// </remarks>
    private static readonly ConcurrentDictionary<Type, object> VoidSendCache = new();

    /// <summary>
    /// A thread-safe, in-memory cache designed to store and retrieve instances of stream handler invokers
    /// associated with different request types. This cache leverages a <see cref="ConcurrentDictionary{TKey, TValue}"/>
    /// for performant and synchronized access across multiple threads.
    /// The primary purpose of <c>StreamCache</c> is to optimize the creation and retrieval of instances
    /// of stream handler invokers, avoiding unnecessary re-creation and ensuring consistent behavior
    /// throughout the lifecycle of the application.
    /// Keys in the cache represent the type of the request, while values are the instances of the
    /// stream handler invokers responsible for handling those request types.
    /// This cache is used within the <see cref="Mediator"/> class to facilitate the `CreateStream` operations.
    /// </summary>
    private static readonly ConcurrentDictionary<Type, object> StreamCache = new();

    /// <summary>
    /// A thread-safe, static cache for storing and managing instances of notification executors
    /// based on their notification type. This is used to optimize the retrieval and reuse of
    /// notification executor instances in the publish pipeline of the mediator.
    /// </summary>
    /// <remarks>
    /// The cache provides a mechanism for efficiently creating and reusing instances of the
    /// <see cref="INotificationExecutor{TNotification}"/> for specific notification types during
    /// the execution of mediator's publish operations.
    /// </remarks>
    private static readonly ConcurrentDictionary<Type, object> PublishCache = new();

    /// <inheritdoc />
    public async ValueTask<TResponse> Send<TResponse>(IRequest<TResponse> request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var requestType = request.GetType();

        var invoker = (IHandlerInvoker<TResponse>)SendCache.GetOrAdd(requestType, static t =>
        {
            var wrapperType = typeof(HandlerInvokerWrapper<,>).MakeGenericType(t, typeof(TResponse));
            return Activator.CreateInstance(wrapperType)!;
        });

        return await hookExecutor.ExecuteWithSendHooks(
            request,
            ct => invoker.Invoke(provider, request, ct),
            cancellationToken);
    }

    /// <inheritdoc />
    public async ValueTask Send(IRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        var type = request.GetType();

        var invoker = (IHandlerInvoker<VoidMarker>)VoidSendCache.GetOrAdd(type, static t =>
        {
            var wrapperType = typeof(HandlerInvokerWrapper<>).MakeGenericType(t);
            return Activator.CreateInstance(wrapperType)!;
        });

        await hookExecutor.ExecuteWithSendHooks(
            request,
            ct => invoker.Invoke(provider, request, ct),
            cancellationToken);
    }

    /// <inheritdoc />
    public async ValueTask<object?> Send(object request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var type = request.GetType();

        // Check if request implements IRequest<T>
        var genericRequestInterface = type
            .GetInterfaces()
            .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequest<>));

        if (genericRequestInterface is not null)
        {
            var responseType = genericRequestInterface.GetGenericArguments()[0];

            var method = typeof(Mediator)
                .GetMethod(nameof(InvokeGenericSend), BindingFlags.NonPublic | BindingFlags.Instance)!
                .MakeGenericMethod(type, responseType);

            return await (ValueTask<object?>)method.Invoke(this, [request, cancellationToken])!;
        }

        // Check if request implements IRequest (non-generic)
        if (request is IRequest nonGeneric)
        {
            return await Send(nonGeneric, cancellationToken)
                .AsTask()
                .ContinueWith(object? (_) => null, cancellationToken)
                .WrapAsValueTask();
        }

        throw new InvalidOperationException($"Invalid request type: {type}");
    }


    /// <inheritdoc />
    public IAsyncEnumerable<TResponse> CreateStream<TResponse>(IStreamRequest<TResponse> request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var type = request.GetType();

        var invoker = (IStreamHandlerInvoker<TResponse>)StreamCache.GetOrAdd(type, static t =>
        {
            var wrapperType = typeof(StreamHandlerInvokerWrapper<,>).MakeGenericType(t, typeof(TResponse));
            return Activator.CreateInstance(wrapperType)!;
        });

        return invoker.Invoke(provider, request, cancellationToken);
    }

    /// <inheritdoc />
    public IAsyncEnumerable<object?> CreateStream(object request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var type = request.GetType();
        var iface = type.GetInterfaces()
                        .FirstOrDefault(i =>
                            i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IStreamRequest<>))
                    ?? throw new InvalidOperationException($"Invalid stream request type: {type}");

        var responseType = iface.GetGenericArguments()[0];

        var method = typeof(Mediator)
            .GetMethod(nameof(InvokeGenericStream), BindingFlags.NonPublic | BindingFlags.Instance)!
            .MakeGenericMethod(type, responseType);

        return (IAsyncEnumerable<object?>)method.Invoke(this, [request, cancellationToken])!;
    }


    /// <inheritdoc />
    public async ValueTask Publish<TNotification>(TNotification notification,
        CancellationToken cancellationToken = default)
        where TNotification : INotification
    {
        ArgumentNullException.ThrowIfNull(notification);

        var type = typeof(TNotification);

        var executor = (INotificationExecutor<TNotification>)PublishCache.GetOrAdd(type, static t =>
        {
            var wrapperType = typeof(NotificationExecutorWrapper<>).MakeGenericType(t);
            return Activator.CreateInstance(wrapperType)!;
        });

        await hookExecutor.ExecuteWithPublishHooks(notification, ct => executor.Execute(provider, notification, ct),
            cancellationToken);
    }

    /// <inheritdoc />
    public async ValueTask Publish(object notification, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(notification);

        var type = notification.GetType();

        _ = type.GetInterfaces().FirstOrDefault(i =>
                i == typeof(INotification) ||
                (i.IsGenericType && i.GetGenericTypeDefinition() == typeof(INotification)))
            ?? throw new InvalidOperationException($"Invalid notification type: {type}");

        var method = typeof(Mediator)
            .GetMethod(nameof(InvokeGenericPublish), BindingFlags.NonPublic | BindingFlags.Instance)!
            .MakeGenericMethod(type);

        await hookExecutor.ExecuteWithPublishHooks(notification,
            ct => (ValueTask)method.Invoke(this, [notification, cancellationToken])!, cancellationToken);
    }

    /// <inheritdoc />
    public async ValueTask<bool> TryPublish(object notification, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(notification);

        try
        {
            await Publish(notification, cancellationToken);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <inheritdoc />
    public async ValueTask<bool> TrySend(object request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        try
        {
            await Send(request, cancellationToken);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Invokes a generic stream operation for the specified request type and response type.
    /// </summary>
    /// <typeparam name="TRequest">The type of the stream request.</typeparam>
    /// <typeparam name="TResponse">The type of the stream response.</typeparam>
    /// <param name="request">The request object implementing <see cref="IStreamRequest{TResponse}"/>.</param>
    /// <param name="ct">The cancellation token used to propagate notification that the operation should be canceled.</param>
    /// <returns>An asynchronous enumerable of objects representing the response stream.</returns>
    private IAsyncEnumerable<object?> InvokeGenericStream<TRequest, TResponse>(TRequest request, CancellationToken ct)
        where TRequest : IStreamRequest<TResponse>
    {
        return CreateStream((IStreamRequest<TResponse>)request, ct).Select(static r => (object?)r);
    }

    /// <summary>
    /// Invokes a generic send operation for a request and response type.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request being handled.</typeparam>
    /// <typeparam name="TResponse">The type of the response expected from the request.</typeparam>
    /// <param name="request">An instance of the request to be processed.</param>
    /// <param name="ct">A token for signaling cancellation of the operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing the result of the send operation as an object.</returns>
    private async ValueTask<object?> InvokeGenericSend<TRequest, TResponse>(TRequest request, CancellationToken ct)
        where TRequest : IRequest<TResponse>
    {
        return await Send((IRequest<TResponse>)request, ct)
            .AsTask()
            .ContinueWith(static t => (object?)t.Result, ct)
            .WrapAsValueTask();
    }

    /// <summary>
    /// Publishes a notification message of a generic type to all relevant handlers.
    /// </summary>
    /// <typeparam name="TNotification">The type of the notification message to be published. Must implement <see cref="INotification"/>.</typeparam>
    /// <param name="notification">The notification object to be published.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    private ValueTask InvokeGenericPublish<TNotification>(TNotification notification, CancellationToken ct)
        where TNotification : INotification
    {
        return Publish(notification, ct);
    }
}