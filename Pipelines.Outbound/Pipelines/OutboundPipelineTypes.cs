namespace Pipelines.Outbound;

public static class OutboundPipelineTypes
{
  const string Scope = $"{nameof(OutboundPipelineTypes)}";
  public const string Persisting = $"{Scope}.{nameof(Persisting)}";
  public const string Publishing = $"{Scope}.{nameof(Publishing)}";
  public const string Dispatching = $"{Scope}.{nameof(Dispatching)}";
}
