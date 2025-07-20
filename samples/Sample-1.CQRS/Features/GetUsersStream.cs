using System.Runtime.CompilerServices;
using Orchestrix.Mediator;

namespace Sample_1.CQRS.Features;

public record GetUsersStream(int Count) : IStreamRequest<UserDto>;

public class GetUsersStreamHandler : IStreamRequestHandler<GetUsersStream, UserDto>
{
    public async IAsyncEnumerable<UserDto> Handle(GetUsersStream request, [EnumeratorCancellation] CancellationToken ct)
    {
        for (int i = 0; i < request.Count; i++)
        {
            yield return new UserDto(Guid.NewGuid(), $"User-{i + 1}");
            await Task.Delay(200, ct);
        }
    }
}