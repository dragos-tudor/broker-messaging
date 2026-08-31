using static Operations.Inbound.DeadLetterEnvelope.DeadLetterEnvelopeStates;

namespace Pipelines.Inbound;

partial class InboundFuncs
{
  internal static string EphemeralDeadLetterEnvelopePipeline(string state) => state switch
  {
    RedirectDeadLetterEnvelopeSuccessState => EphemeralDeadLetterEnvelopeActions.Redirected,
    RedirectDeadLetterEnvelopeErrorState => EphemeralDeadLetterEnvelopeActions.CheckingRetry,

    CheckRetryDeadLetterEnvelopeExhaustedState => EphemeralDeadLetterEnvelopeActions.CheckedRetry,
    CheckRetryDeadLetterEnvelopeNotExhaustedState => EphemeralDeadLetterEnvelopeActions.UpsertingRetry,
    CheckRetryDeadLetterEnvelopeErrorState => EphemeralDeadLetterEnvelopeActions.CheckingRetry,

    UpsertRetryDeadLetterEnvelopeSuccessState => EphemeralDeadLetterEnvelopeActions.Deferring,
    UpsertRetryDeadLetterEnvelopeErrorState => EphemeralDeadLetterEnvelopeActions.UpsertingRetry,

    _ => EphemeralDeadLetterEnvelopeActions.Unknown
  };

 internal static string DeadLetterEnvelopePipeline(string state) => state switch
  {
    PublishDeadLetterEnvelopeSuccessState => DeadLetterEnvelopeActions.Published,
    PublishDeadLetterEnvelopeErrorState => DeadLetterEnvelopeActions.Publishing,

    ProducingDeadLetterEnvelopeState => DeadLetterEnvelopeActions.Deferring,
    ProduceDeadLetterEnvelopeErrorState => DeadLetterEnvelopeActions.Producing,

    _ => DeadLetterEnvelopeActions.Unknown
  };
}


internal static class EphemeralDeadLetterEnvelopeActions
{
  private const string Scope = "EphemeralDeadLetterEnvelope";

  public const string Redirecting = $"{Scope}.{nameof(Redirecting)}";
  public const string Redirected = $"{Scope}.{nameof(Redirected)}";
  public const string CheckingRetry = $"{Scope}.{nameof(CheckingRetry)}";
  public const string CheckedRetry = $"{Scope}.{nameof(CheckedRetry)}";
  public const string UpsertingRetry = $"{Scope}.{nameof(UpsertingRetry)}";
  public const string Deferring = $"{Scope}.{nameof(Deferring)}";
  public const string Unknown = $"{Scope}.{nameof(Unknown)}";
}

internal static class DeadLetterEnvelopeActions
{
  private const string Scope = "DeadLetterEnvelope";

  public const string Publishing = $"{Scope}.{nameof(Publishing)}";
  public const string Published = $"{Scope}.{nameof(Published)}";
  public const string Producing = $"{Scope}.{nameof(Producing)}";
  public const string Deferring = $"{Scope}.{nameof(Deferring)}";
  public const string Unknown = $"{Scope}.{nameof(Unknown)}";
}