
global using AutoFixture;
global using AutoFixture.AutoNSubstitute;
global using Microsoft.VisualStudio.TestTools.UnitTesting;
global using NSubstitute;
global using NSubstitute.ExceptionExtensions;
global using Shouldly;

namespace Operations.Inbound.DeadLetterEnvelope;

[TestClass]
public partial class DeadLetterEnvelopeTests
{
  static object?[] CreateEnvelopeData<TKey, TValue, TMetadata, TConfirmation>(
    IDeadLetterEnvelope<TKey, TValue, TMetadata, TConfirmation>? envelope)
  {
    object?[] data = new object?[6];
    if (envelope is not null)
      SetDeadLetterEnvelope(data, envelope);
    return data;
  }

  static object?[] CreateProducingData<TKey, TValue, TMetadata, TConfirmation, TPayload>(
    IDeadLetterEnvelope<TKey, TValue, TMetadata, TConfirmation>? envelope,
    IDeadLetterMessage<TKey, TPayload>? message)
  {
    object?[] data = new object?[6];
    if (envelope is not null)
      SetDeadLetterEnvelope(data, envelope);
    if (message is not null)
      SetDeadLetterMessage(data, message);
    return data;
  }

  static object?[] CreateDispatchingData(ProduceResult? result)
  {
    object?[] data = new object?[6];
    if (result is not null)
      DeadLetterEnvelopeFuncs.SetProduceResult(data, result);
    return data;
  }

  static readonly IFixture Fixture = new Fixture().Customize(
    new AutoNSubstituteCustomization
    {
      ConfigureMembers = true,
      GenerateDelegates = true
    });
}
