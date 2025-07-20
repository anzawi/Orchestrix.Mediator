using Orchestrix.Mediator;
using Sample_1.Features;
using Sample_1.Infrastructure;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddOrchestrix(cfg =>
{
    cfg.RegisterHandlersFromAssemblies(typeof(CreateUserCommand).Assembly);
    cfg.AddHooksFromAssemblies(typeof(LoggingHook).Assembly);
    cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); 
builder.Services.AddControllers();
var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sample 1 with CQRS API v1");
    c.RoutePrefix = "swagger";
});


app.UseHttpsRedirection();
app.MapControllers();

app.Run();