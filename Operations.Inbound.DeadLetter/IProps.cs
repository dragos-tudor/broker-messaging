
namespace Operations.Inbound.DeadLetter;

public interface IDeadLetterEnvelopeProp<TKey, TValue, TMetadata, TConfirmation>
{
  IDeadLetterEnvelope<TKey, TValue, TMetadata, TConfirmation>? DeadLetterEnvelope { get; set; }
}

public interface IDeadLetterMessageProp<TKey, TPayload>
{
  DeadLetterMessage<TKey, TPayload>? DeadLetterMessage { get; set; }
}

public interface IPipelineErrorProp { string PipelineError { get; set; } }

partial class DeadLetterFuncs
{
  static DeadLetterMessage<TKey, TPayload> RequireDeadLetterMessage<TKey, TPayload>(
    DeadLetterMessage<TKey, TPayload>? message) =>
    message ?? throw new InvalidOperationException("Dead letter message is required.");
}