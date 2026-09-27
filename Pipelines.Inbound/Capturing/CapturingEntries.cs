
namespace Pipelines.Inbound;

internal static class CapturingEntries {
  const string Scope = $"{nameof(CapturingEntries)}";
  internal const string Start = $"{Scope}.{nameof(Start)}";
  internal const string End = $"{Scope}.{nameof(End)}";
}