namespace Pipelines.Outbound;

partial class OutboundFuncs
{
  static Task<TResult> ToTask<TResult>(TResult result) =>
    Task.FromResult(result);
}
