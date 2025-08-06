using Orchestrix.Mediator;
using Orchestrix.Mediator.Sagas.Contracts;
using Sample_InProcess_Saga.Models;

namespace Sample_InProcess_Saga.Sagas;


public sealed class ReserveInventoryStep : ISagaStep<PlaceOrderContext>
{
    public ValueTask ExecuteAsync(PlaceOrderContext input, ISender sender, CancellationToken ct)
    {
        // we can here send command
        // await sender.Send(new ReserveInventoryCommand(input.CustomerId), ct);
        Console.WriteLine($"[Step] Reserving inventory for product {input.ProductId}");

        if (input.Quantity <= 0)
            throw new InvalidOperationException("Quantity must be greater than 0");

        return default;
    }
}