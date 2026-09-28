namespace Pipelines.Outbound;

partial class OutboundFuncs
{
  internal static Task<TResult> ToTask<TResult>(TResult result) =>
    Task.FromResult(result);
}
