using Orchestrix.Mediator;
using Orchestrix.Mediator.Sagas.Contracts;
using Sample_InProcess_Saga.Models;

namespace Sample_InProcess_Saga.Sagas;


public sealed class ValidateCustomerStep : ISagaStep<PlaceOrderContext>
{
    public ValueTask ExecuteAsync(PlaceOrderContext input, ISender sender, CancellationToken ct)
    {
        // we can here send command
        // await sender.Send(new ValidateCustomerCommand(input.CustomerId), ct);
        Console.WriteLine($"[Step] Validating customer {input.CustomerId}");

        if (input.CustomerId == Guid.Empty)
            throw new InvalidOperationException("Invalid customer");

        return default;
    }
}