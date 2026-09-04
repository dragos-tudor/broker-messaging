
namespace Pipelines.Inbound;

partial class InboundFuncs
{
  internal static string? GetInboundPipelineAction(string state) =>
    GetEnvelopePipelineAction(state) ??
    GetEphemeralDeadLetterEnvelopePipelineAction(state) ??
    GetInboxPipelineAction(state) ??
    GetDeadLetterPipelineAction(state) ??
    GetDeadLetterEnvelopePipelineAction(state);
}