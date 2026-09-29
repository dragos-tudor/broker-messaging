
namespace Pipelines.Outbound;

public record struct PipelineContext(
  Guid? CorrelationId,
  Guid? OutboxMessageId
);

partial class OutboundFuncs
{
  internal static PipelineContext CreatePipelineContext(object?[] data) =>
    new (
      GetCorrelationId(data),
      GetOutboxMessageId(data)
    );

  static Guid? GetCorrelationId(object?[] data) =>
    GetOutboxMessage(data)?.CorrelationId;

  static Guid? GetOutboxMessageId(object?[] data) =>
    GetOutboxMessage(data)?.MessageId;
}