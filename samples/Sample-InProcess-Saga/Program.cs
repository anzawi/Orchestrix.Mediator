using Microsoft.AspNetCore.Mvc;
using Orchestrix.Mediator;
using Orchestrix.Mediator.Sagas.Contracts;
using Orchestrix.Mediator.Sagas.Extensions;
using Sample_InProcess_Saga.Decorators;
using Sample_InProcess_Saga.Hooks;
using Sample_InProcess_Saga.Models;
using Sample_InProcess_Saga.Sagas;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();


// Here to add the sagas
builder.Services.AddOrchestrix(cfg => cfg
    .RegisterHandlersFromAssemblies(typeof(Test).Assembly)
    .AddSagas(typeof(PlaceOrderSaga).Assembly)
    .AddSagaHook<LoggingHook>()
    .AddSagaDecorator<TimingDecorator>()
);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/", async ([FromServices] ISagaCoordinator sagaCoordinator) =>
    {
        var input = new PlaceOrderContext(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            3
        );
        await sagaCoordinator.ExecuteAsync(input);
        return input;
    })
    .WithName("Test Saga");

app.Run();