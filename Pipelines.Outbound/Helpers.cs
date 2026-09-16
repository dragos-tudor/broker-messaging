namespace Pipelines.Outbound;

partial class OutboundFuncs
{
  internal static async ValueTask<(TData, TSignal, Exception?)> FromResult<TData, TState, TSignal>(
    this ValueTask<(TData, TState, Exception?)> result,
    Func<TState, TSignal> cast)
  {
    var (data, state, exception) = await result;
    return (data, cast(state), exception);
  }
}
