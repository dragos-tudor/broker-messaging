
namespace Pipelines.Inbound;

partial class InboundFuncs
{
  static Task<TResult> ToTask<TResult>(TResult result) =>
    Task.FromResult(result);
}
