
global using AutoFixture;
global using AutoFixture.AutoNSubstitute;
global using Microsoft.VisualStudio.TestTools.UnitTesting;
global using NSubstitute;
global using NSubstitute.ExceptionExtensions;
global using Shouldly;

namespace Operations.Outbound.Envelope;

[TestClass]
public partial class EnvelopeTests
{
  static object?[] CreateEnvelopeData<TKey, TValue, TMetadata, TConfirmation>(
    IEnvelope<TKey, TValue, TMetadata, TConfirmation>? envelope)
  {
    object?[] data = new object?[3];
    if (envelope is not null)
      SetEnvelope(data, envelope);
    return data;
  }

  static object?[] CreateProducingData<TKey, TValue, TMetadata, TConfirmation, TPayload>(
    IEnvelope<TKey, TValue, TMetadata, TConfirmation>? envelope,
    IOutboxMessage<TKey, TPayload>? message)
  {
    object?[] data = new object?[3];
    if (envelope is not null)
      SetEnvelope(data, envelope);
    if (message is not null)
      SetOutboxMessage(data, message);
    return data;
  }

  static object?[] CreateDispatchingData(ProduceResult? result)
  {
    object?[] data = new object?[3];
    if (result is not null)
      EnvelopeFuncs.SetProduceResult(data, result);
    return data;
  }

  static readonly IFixture Fixture = new Fixture().Customize(
    new AutoNSubstituteCustomization
    {
      ConfigureMembers = true,
      GenerateDelegates = true
    });
}
