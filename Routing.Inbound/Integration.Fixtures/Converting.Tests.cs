
namespace Routing.Inbound;

partial class IntegrationTests
{
  static Enum FromSignal<TSignal>(TSignal signal) =>
    signal switch
    {
      CapturingSignal s => (s.Value as Enum)!,
      RedirectingSignal s => (s.Value as Enum)!,
      HandlingSignal s => (s.Value as Enum)!,
      DeadLetteringSignal s => (s.Value as Enum)!,
      PublishingSignal s => (s.Value as Enum)!,
      DispatchingSignal s => (s.Value as Enum)!,
      _ => throw new ArgumentOutOfRangeException(signal!.ToString())
    };

  static Enum FromDecision<TDecision>(TDecision decision) =>
    decision switch
    {
      CapturingDecision d => (d.Value as Enum)!,
      RedirectingDecision d => (d.Value as Enum)!,
      HandlingDecision d => (d.Value as Enum)!,
      DeadLetteringDecision d => (d.Value as Enum)!,
      PublishingDecision d => (d.Value as Enum)!,
      DispatchingDecision d => (d.Value as Enum)!,
      _ => throw new ArgumentOutOfRangeException(decision!.ToString())
    };
}
