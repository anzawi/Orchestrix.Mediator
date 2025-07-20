using Orchestrix.Mediator.Cqrs;

namespace Sample_1.CQRS.Features;

public record UserDto(Guid Id, string Name);
public record GetUserQuery(Guid Id) : IQuery<UserDto>;

public class GetUserHandler : IQueryHandler<GetUserQuery, UserDto>
{
    public ValueTask<UserDto> Handle(GetUserQuery request, CancellationToken ct)
    {
        return ValueTask.FromResult(new UserDto(request.Id, "Test User"));
    }
}