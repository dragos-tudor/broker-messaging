
namespace Pipelines.Inbound;

public record struct PipelineContext(
  string? TransportId,
  Guid? CorrelationId,
  Guid? InboxMessageId,
  Guid? DeadLetterMessageId
);

partial class InboundFuncs
{
  internal static PipelineContext CreatePipelineContext(object?[] data) =>
    new (
      GetTransportMessageId(data),
      GetCorrelationId(data),
      GetInboxMessageId(data),
      GetDeadLetterMessageId(data)
    );

  static Guid? GetCorrelationId(object?[] data) =>
    GetInboxMessage(data)?.CorrelationId ??
    GetDeadLetterMessage(data)?.CorrelationId;

  static Guid? GetDeadLetterMessageId(object?[] data) =>
    GetDeadLetterMessage(data)?.MessageId;

  static Guid? GetInboxMessageId(object?[] data) =>
    GetInboxMessage(data)?.MessageId;

  static string? GetTransportMessageId(object?[] data) =>
    GetEnvelope(data)?.TransportMessageId ??
    GetInboxMessage(data)?.TransportMessageId ??
    GetDeadLetterMessage(data)?.TransportMessageId ??
    GetDeadLetterEnvelope(data)?.OriginalTransportMessageId;
}