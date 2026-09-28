namespace Pipelines.Outbound;

public static class PipelineTypes
{
  internal const string Scope = $"{nameof(PipelineTypes)}";
  public const string Persisting = $"{Scope}.{nameof(Persisting)}";
  public const string Publishing = $"{Scope}.{nameof(Publishing)}";
  public const string Dispatching = $"{Scope}.{nameof(Dispatching)}";
}

partial class OutboundFuncs
{
  internal static bool IsPipelineType(string decision) =>
    decision.StartsWith(PipelineTypes.Scope, StringComparison.OrdinalIgnoreCase);
}