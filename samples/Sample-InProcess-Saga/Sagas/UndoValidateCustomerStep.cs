using Orchestrix.Mediator;
using Orchestrix.Mediator.Sagas.Contracts;
using Sample_InProcess_Saga.Models;

namespace Sample_InProcess_Saga.Sagas;


public sealed class UndoValidateCustomerStep : ISagaCompensationStep<PlaceOrderContext>
{
    public ValueTask CompensateAsync(PlaceOrderContext input, ISender sender, CancellationToken ct)
    {
        // we can here send command
        // await sender.Send(new UndoValidateCustomerCommand(input.CustomerId), ct);
        Console.WriteLine($"[Compensate] Undo validation for customer {input.CustomerId}");
        return default;
    }
}