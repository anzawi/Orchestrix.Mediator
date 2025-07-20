using Orchestrix.Mediator;

namespace Sample_1.Features;

public record UserDto(Guid Id, string Name);
public record GetUserQuery(Guid Id) : IRequest<UserDto>;

public class GetUserHandler : IRequestHandler<GetUserQuery, UserDto>
{
    public ValueTask<UserDto> Handle(GetUserQuery request, CancellationToken ct)
    {
        return ValueTask.FromResult(new UserDto(request.Id, "Test User"));
    }
}