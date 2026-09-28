
namespace Routing.Inbound;

internal readonly record struct RoutingResult(
  string PipelineType,
  string? Signal,
  string Decision
);

partial class InboundFuncs
{
  static RoutingResult CreateRoutingResult(
    string pipelineType,
    string? signal,
    string decision) =>
      new (pipelineType, signal, decision);
}