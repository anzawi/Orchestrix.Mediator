using Orchestrix.Mediator;

namespace Sample_InProcess_Saga.Models;

public sealed record PlaceOrderContext(
    Guid OrderId,
    Guid CustomerId,
    Guid ProductId,
    int Quantity
);


public sealed record Test: IRequest;

public sealed record TestHandler : IRequestHandler<Test>
{
    public ValueTask Handle(Test request, CancellationToken ct)
    {
        return default;
    }
}