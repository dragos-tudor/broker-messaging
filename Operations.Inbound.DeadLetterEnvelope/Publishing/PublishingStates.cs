
namespace Operations.Inbound.DeadLetterEnvelope;

internal static class PublishingStates
{
  const string Scope = $"{nameof(PublishingStates)}";
  internal const string Success = $"{Scope}.{nameof(Success)}";
  internal const string Error = $"{Scope}.{nameof(Error)}";
}
