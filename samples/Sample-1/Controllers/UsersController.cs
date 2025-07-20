using Microsoft.AspNetCore.Mvc;
using Orchestrix.Mediator;
using Sample_1.Features;

namespace Sample_1.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController(IMediator mediator) : ControllerBase
{
    [HttpPost("create")]
    public async Task<IActionResult> Create(CreateUserCommand command)
        => Ok(await mediator.Send(command));

    [HttpPost("create-void")]
    public async Task<IActionResult> Create(CreateUserCommandVoid command)
    {
        await mediator.Send(command);
        return Ok("Created");
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
        => Ok(await mediator.Send(new GetUserQuery(id)));

    [HttpGet("stream")]
    public async IAsyncEnumerable<UserDto> Stream([FromQuery] int count, [FromServices] ISender sender)
    {
        await foreach (var user in sender.CreateStream(new GetUsersStream(count)))
            yield return user;
    }
}