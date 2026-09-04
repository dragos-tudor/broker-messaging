
namespace Pipelines.Inbound;

static class EphemeralDeadLetterEnvelopeActions
{
  internal const string Scope = "EphemeralDeadLetterEnvelope";
  internal const string Redirecting = $"{Scope}.{nameof(Redirecting)}";
  internal const string Redirected = $"{Scope}.{nameof(Redirected)}";
  internal const string CheckingRetry = $"{Scope}.{nameof(CheckingRetry)}";
  internal const string RetryExhausted = $"{Scope}.{nameof(RetryExhausted)}";
  internal const string RegisteringRetry = $"{Scope}.{nameof(RegisteringRetry)}";
}

static class DeadLetterEnvelopeActions
{
  internal const string Scope = "DeadLetterEnvelope";
  internal const string Publishing = $"{Scope}.{nameof(Publishing)}";
  internal const string Published = $"{Scope}.{nameof(Published)}";
  internal const string Producing = $"{Scope}.{nameof(Producing)}";
}