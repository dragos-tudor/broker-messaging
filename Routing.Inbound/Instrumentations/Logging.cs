
using Microsoft.Extensions.Logging;

namespace Routing.Inbound;

partial class InboundFuncs
{
  [LoggerMessage(1, LogLevel.Information, "Pipeline: signal {signal}, decision {decision}, transportId {transportId}, correlationId {correlationId}, inboxMessageId {inboxMessageId}, deadLetterMessageId {deadLetterMessageId}.")]
  static partial void LogPipeline(ILogger logger, string signal, string decision,
    string? transportId, Guid? correlationId, Guid? inboxMessageId, Guid? deadLetterMessageId);

  [LoggerMessage(2, LogLevel.Information, "Operation:  signal {signal}, transportId {transportId}, correlationId {correlationId}, inboxMessageId {inboxMessageId}, deadLetterMessageId {deadLetterMessageId}.")]
  static partial void LogOperation(ILogger logger, string signal,
    string? transportId, Guid? correlationId, Guid? inboxMessageId, Guid? deadLetterMessageId);

  [LoggerMessage(3, LogLevel.Error, "Operation:  signal {signal},transportId {transportId}, correlationId {correlationId}, inboxMessageId {inboxMessageId}, deadLetterMessageId {deadLetterMessageId}.")]
  static partial void LogOperationError(ILogger logger, string signal,
    string? transportId, Guid? correlationId, Guid? inboxMessageId, Guid? deadLetterMessageId, Exception exception);
}