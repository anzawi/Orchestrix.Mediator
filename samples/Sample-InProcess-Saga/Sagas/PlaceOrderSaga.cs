using Orchestrix.Mediator.Sagas.FluentApi;
using Sample_InProcess_Saga.Models;

namespace Sample_InProcess_Saga.Sagas;

public sealed class PlaceOrderSaga : SagaBuilder<PlaceOrderContext>
{
    protected override void Build(SagaStepBuilder<PlaceOrderContext> saga)
    {
        saga.Step<ValidateCustomerStep>()
            .Retry(2)
            .Compensate<UndoValidateCustomerStep>();

        saga.Step<ReserveInventoryStep>()
            .Retry(1)
            .Timeout(TimeSpan.FromSeconds(5));
    }
}