
namespace Pipelines.Inbound;

partial class InboundFuncs
{
  internal static Task<TResult> ToTask<TResult>(TResult result) =>
    Task.FromResult(result);
}
