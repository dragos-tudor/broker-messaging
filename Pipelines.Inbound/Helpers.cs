
namespace Pipelines.Inbound;

partial class InboundFuncs
{
  internal static async ValueTask<(TData, TInput, Exception?)> FromResult<TData, TState, TInput>(
    this ValueTask<(TData, TState, Exception?)> result,
    Func<TState, TInput> cast)
  {
    var (data, state, exception) = await result;
    return (data, cast(state), exception);
  }
}