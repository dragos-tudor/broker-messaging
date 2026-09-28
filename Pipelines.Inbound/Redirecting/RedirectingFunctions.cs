
namespace Pipelines.Inbound;

partial class InboundFuncs
{
  static PropagateException NoPropagationException = (_, __, ___) => string.Empty;

  internal static PipelineFunctions<RedirectingCapabilities>
    CreateRedirectingFunctions() =>
      new (
        AdvanceRedirectingPipeline,
        ExecuteRedirectingOperationAsync,
        NoPropagationException,
        CanFastRetryRedirecting
      );
}