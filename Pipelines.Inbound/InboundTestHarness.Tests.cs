using System.Collections.Generic;
using Persistence.DeadLetterMessage;
using Persistence.InboxMessage;
using Persistence.RetryMessage;
using Transport.DeadLetterEnvelope;
using Transport.Envelope;

namespace Pipelines.Inbound;

public interface IInboundServices<TKey, TValue, TMetadata, TConfirmation, TPayload, TSession> :
  IDeadLetterServices<TKey, TValue, TMetadata, TConfirmation, TPayload>,
  IDeadLetterEnvelopeServices<TKey, TValue, TMetadata, TConfirmation, TPayload>,
  IEnvelopeServices<TKey, TValue, TMetadata, TConfirmation, TPayload>,
  IInboxServices<TKey, TValue, TMetadata, TConfirmation, TPayload, TSession>
  where TSession : IDisposable;

public interface IInboundData<TKey, TValue, TMetadata, TConfirmation, TPayload> :
  IDeadLetterData<TKey, TValue, TMetadata, TConfirmation, TPayload>,
  IDeadLetterEnvelopeData<TKey, TValue, TMetadata, TConfirmation, TPayload>,
  IEnvelopeData<TKey, TValue, TMetadata, TConfirmation, TPayload>,
  IInboxData<TKey, TValue, TMetadata, TConfirmation, TPayload>
{
  new InboxMessage<TKey, TPayload>? InboxMessage { get; set; }
  new DeadLetterMessage<TKey, TPayload>? DeadLetterMessage { get; set; }
  new IDeadLetterEnvelope<TKey, TValue, TMetadata, TConfirmation>? DeadLetterEnvelope { get; set; }
  new string PipelineError { get; set; }
}

public class InboundData<TKey, TValue, TMetadata, TConfirmation, TPayload> :
  IInboundData<TKey, TValue, TMetadata, TConfirmation, TPayload>
{
  public IDeadLetterEnvelope<TKey, TValue, TMetadata, TConfirmation>? DeadLetterEnvelope { get; set; }
  public string PipelineError { get; set; } = string.Empty;
  string? Operations.Inbound.Envelope.IPipelineErrorProp.PipelineError
  {
    get => PipelineError;
    set => PipelineError = value ?? string.Empty;
  }
  public DeadLetterMessage<TKey, TPayload>? DeadLetterMessage { get; set; }
  public RetryMessage? RetryMessage { get; set; }
  public InboxMessage<TKey, TPayload>? InboxMessage { get; set; }
  public IEnvelope<TKey, TValue, TMetadata, TConfirmation>? Envelope { get; set; }
  public object? Model { get; set; }
}

public static class InboundTestHarness
{
  public static string GetPipelineAction(string state)
  {
    var ephem = InboundFuncs.EphemeralDeadLetterEnvelopePipeline(state);
    if (ephem != EphemeralDeadLetterEnvelopeActions.Unknown) return ephem;

    var dle = InboundFuncs.DeadLetterEnvelopePipeline(state);
    if (dle != DeadLetterEnvelopeActions.Unknown) return dle;

    var env = InboundFuncs.EnvelopePipeline(state);
    if (env != EnvelopeActions.Unknown) return env;

    var inb = InboundFuncs.InboxPipeline(state);
    if (inb != InboxActions.Unknown) return inb;

    var dl = InboundFuncs.DeadLetterPipeline(state);
    if (dl != DeadLetterActions.Unknown) return dl;

    return "Unknown";
  }

  public static Func<TServices, TData, CancellationToken, ValueTask<(TData, string, Exception?)>>?
    GetInboundOperation<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload, TSession>(string action)
    where TServices : IInboundServices<TKey, TValue, TMetadata, TConfirmation, TPayload, TSession>
    where TData : IInboundData<TKey, TValue, TMetadata, TConfirmation, TPayload>
    where TSession : IDisposable =>
    action.Split(".")[0] switch
    {
      "Envelope" => InboundFuncs.GetEnvelopeOperation<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>(action),
      "Inbox" => InboundFuncs.GetInboxOperation<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload, TSession>(action),
      "DeadLetter" => InboundFuncs.GetDeadLetterOperation<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>(action),
      "EphemeralDeadLetterEnvelope" => InboundFuncs.GetEphemeralDeadLetterEnvelopeOperation<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>(action),
      "DeadLetterEnvelope" => InboundFuncs.GetDeadLetterEnvelopeOperation<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>(action),
      _ => null
    };

  public static async Task<(List<string> States, List<string> Actions)>
    RunInboundPipelineAsync<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload, TSession>(
      TServices services,
      TData data,
      string initialAction,
      InboundPipelineConfig config = default,
      CancellationToken ct = default)
    where TServices : IInboundServices<TKey, TValue, TMetadata, TConfirmation, TPayload, TSession>
    where TData : IInboundData<TKey, TValue, TMetadata, TConfirmation, TPayload>
    where TSession : IDisposable
  {
    var states = new List<string>();
    var actions = new List<string>();

    var currentAction = initialAction;
    actions.Add(currentAction);

    while (true)
    {
      var operation = GetInboundOperation<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload, TSession>(currentAction);
      if (operation == null) break;

      var (nextData, state, exception) = await operation(services, data, ct);
      data = nextData;
      states.Add(state);

      var localAction = GetPipelineAction(state);
      if (localAction == "Unknown") break;
      actions.Add(localAction);

      var nextAction = InboundFuncs.MapInboundAction(localAction, config) ?? localAction;

      // Router Guard: if next action is Inbox Handling, ensure InboxMessage status is Processing
      if (nextAction == InboxActions.Handling && (data.InboxMessage == null || data.InboxMessage.Status != InboxMessageStatus.Processing))
      {
        break;
      }

      if (nextAction != localAction)
      {
        actions.Add(nextAction);
      }

      // If action did not change or maps to self without dispatchable operation, stop
      if (nextAction == currentAction && GetInboundOperation<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload, TSession>(nextAction) == null)
      {
        break;
      }

      currentAction = nextAction;
    }

    return (states, actions);
  }
}

