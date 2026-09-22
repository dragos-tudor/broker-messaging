namespace Pipelines.Outbound;

partial class OutboundFuncs
{
  static async Task<(TData, TSignal, Exception?)> FromResult<TData, TState, TSignal>(
    this Task<(TData, TState, Exception?)> result,
    Func<TState, TSignal> cast)
  {
    var (data, state, exception) = await result;
    return (data, cast(state), exception);
  }

  static Task<(TData, TSignal, Exception?)> FromResult<TData, TState, TSignal>(
    this (TData, TState, Exception?) result,
    Func<TState, TSignal> cast)
  {
    var (data, state, exception) = result;
    return Task.FromResult((data, cast(state), exception));
  }

  static Task<(TData, TSignal, Exception?)> ToResult<TData, TSignal>(TData data, TSignal signal) =>
    Task.FromResult((data, signal, default(Exception?)));
}
