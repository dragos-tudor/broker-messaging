
namespace Foundation.Extensions;

public interface ISessionService : IDisposable
{
  Task CompleteAsync(CancellationToken ct = default);
}