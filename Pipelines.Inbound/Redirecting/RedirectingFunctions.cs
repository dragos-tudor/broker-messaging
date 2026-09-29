
namespace Pipelines.Inbound;

partial class InboundFuncs
{
  static readonly PropagateException NoPropagationException = (_, __, ___) => string.Empty;

  internal static PipelineFunctions<RedirectingCapabilities>
    CreateRedirectingFunctions() =>
      new (
        AdvanceRedirectingPipeline,
        ExecuteRedirectingOperationAsync,
        NoPropagationException,
        CanFastRetryRedirecting
      );
}