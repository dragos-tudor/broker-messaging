
namespace Pipelines.Inbound;

partial class InboundFuncs
{
  internal static string? MapInboundPipeline(string? action, InboundMappingConfig config = default) =>
    action switch
    {
      // 1. Envelope Pipeline actions
      EnvelopeActions.Mapped => InboxActions.Validating,
      EnvelopeActions.Converted => EphemeralDeadLetterEnvelopeActions.Redirecting,
      EnvelopeActions.Confirmed => config.ShouldHandleMessage ? InboxActions.Handling : default,

      // 2. Ephemeral DeadLetterEnvelope Pipeline actions (in-memory redirect path)
      EphemeralDeadLetterEnvelopeActions.Redirected => EnvelopeActions.Confirming,
      EphemeralDeadLetterEnvelopeActions.RetryExhausted => EnvelopeActions.Confirming,

      // 3. Inbox Pipeline actions
      InboxActions.Inserted => EnvelopeActions.Confirming,
      InboxActions.Idempotent => EnvelopeActions.Confirming,
      InboxActions.RetryExhausted => EnvelopeActions.Confirming,
      InboxActions.Converted => DeadLetterActions.Inserting,

      // 4. DeadLetter Pipeline actions (persisted path)
      DeadLetterActions.Mapped => config.UseBrokerPublisher
        ? DeadLetterEnvelopeActions.Publishing
        : DeadLetterEnvelopeActions.Producing,
      DeadLetterActions.Closed => InboxActions.Closing,

      // 5. DeadLetterEnvelope Pipeline actions (persisted path)
      DeadLetterEnvelopeActions.Published => DeadLetterActions.Closing,

      // Terminal, deferred, and cross-pipeline actions produce no next action
      _ => default
    };
}