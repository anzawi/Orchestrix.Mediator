namespace Orchestrix.Mediator.Sagas.Execution;

public sealed class SagaExecutionException(string message, Exception inner) : Exception(message, inner);