using System.Runtime.CompilerServices;
using Orchestrix.Mediator;

namespace Sample_2.SourceGenerator.Features;

public record GetUsersStream(int Count) : IStreamRequest<string>;

public class GetUsersStreamHandler : IStreamRequestHandler<GetUsersStream, string>
{
    public async IAsyncEnumerable<string> Handle(GetUsersStream request, [EnumeratorCancellation] CancellationToken ct)
    {
        for (int i = 1; i <= request.Count; i++)
        {
            yield return $"User-{i}";
            await Task.Delay(150, ct);
        }
    }
}