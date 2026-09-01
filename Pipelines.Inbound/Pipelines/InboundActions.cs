
namespace Pipelines.Inbound;

partial class InboundFuncs
{
  internal static string? GetPipelineAction(string state) =>
    GetEphemeralDeadLetterEnvelopePipelineAction(state) ??
    GetDeadLetterEnvelopePipelineAction(state) ??
    GetEnvelopePipelineAction(state) ??
    GetInboxPipelineAction(state) ??
    GetDeadLetterPipelineAction(state);
}