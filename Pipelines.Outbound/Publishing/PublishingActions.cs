namespace Pipelines.Outbound;

internal static class PublishingActions
{
  const string Scope = $"{nameof(PublishingActions)}";
  internal const string Mapping = $"{Scope}.{nameof(Mapping)}";
  internal const string Publishing = $"{Scope}.{nameof(Publishing)}";
  internal const string Producing = $"{Scope}.{nameof(Producing)}";
  internal const string Scheduling = $"{Scope}.{nameof(Scheduling)}";
  internal const string Abandoning = $"{Scope}.{nameof(Abandoning)}";
  internal const string Closing = $"{Scope}.{nameof(Closing)}";
}
