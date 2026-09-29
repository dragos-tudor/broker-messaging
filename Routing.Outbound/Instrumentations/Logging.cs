using Microsoft.Extensions.Logging;

namespace Routing.Outbound;

partial class OutboundFuncs
{
  [LoggerMessage(11, LogLevel.Information, "Pipeline: signal {signal}, decision {decision}, correlationId {correlationId}, outboxMessageId {outboxMessageId}.")]
  static partial void LogPipeline(
    ILogger logger, string signal, string decision,
    Guid? correlationId, Guid? outboxMessageId);

  [LoggerMessage(12, LogLevel.Information, "Operation:  signal {signal}, correlationId {correlationId}, outboxMessageId {outboxMessageId}.")]
  static partial void LogOperation(
    ILogger logger, string signal,
    Guid? correlationId, Guid? outboxMessageId);

  [LoggerMessage(13, LogLevel.Error, "Operation:  signal {signal}, correlationId {correlationId}, outboxMessageId {outboxMessageId}.")]
  static partial void LogOperationError(
    ILogger logger, string signal,
    Guid? correlationId, Guid? outboxMessageId, Exception exception);
}
