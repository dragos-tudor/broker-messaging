namespace Operations.Inbound.Envelope;

public partial class EnvelopeTests
{
  [TestMethod]
  public void map_envelope__mapper_returns_message__returns_success_and_sets_data()
  {
    var capabilities =
      Fixture.Create<MappingCapabilities<string, byte[], object, string, string>>();

    var envelope =
      Fixture.Create<IEnvelope<string, byte[], object, string>>();

    var message =
      Fixture.Create<IInboxMessage<string, string>>();

    var currentDate = Fixture.Create<DateTime>();
    var inputData =
      new MappingData<string, byte[], object, string, string>(envelope, null);

    capabilities.GetUtcDateTime().Returns(currentDate);
    capabilities.FromEnvelope(
        inputData.Envelope!,
        currentDate,
        InboxMessageStatus.Processing)
      .Returns(message);

    var (data, state, exception) =
      EnvelopeFuncs.MapEnvelope(capabilities, inputData);

    data.InboxMessage.ShouldBe(message);
    state.ShouldBe(MappingStates.Success);
    exception.ShouldBeNull();

    capabilities.GetUtcDateTime.Received(1)();
    capabilities.FromEnvelope.Received(1)(
      inputData.Envelope!,
      currentDate,
      InboxMessageStatus.Processing);
  }

  [TestMethod]
  public void map_envelope__envelope_missing__returns_error_with_exception()
  {
    var capabilities =
      Fixture.Create<MappingCapabilities<string, byte[], object, string, string>>();

    var inputData =
      new MappingData<string, byte[], object, string, string>(null, null);

    var (data, state, exception) =
      EnvelopeFuncs.MapEnvelope(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(MappingStates.Error);
    exception.ShouldBeOfType<InvalidOperationException>();

    capabilities.FromEnvelope.Received(0)(
      default!,
      default,
      InboxMessageStatus.Processing);
  }

  [TestMethod]
  public void map_envelope__mapper_throws__returns_error_with_exception()
  {
    var capabilities =
      Fixture.Create<MappingCapabilities<string, byte[], object, string, string>>();

    var envelope =
      Fixture.Create<IEnvelope<string, byte[], object, string>>();

    var currentDate = Fixture.Create<DateTime>();
    var expectedException = new InvalidOperationException("mapping failed");
    var inputData =
      new MappingData<string, byte[], object, string, string>(envelope, null);

    capabilities.GetUtcDateTime().Returns(currentDate);
    capabilities.FromEnvelope(
        inputData.Envelope!,
        currentDate,
        InboxMessageStatus.Processing)
      .Throws(expectedException);

    var (data, state, exception) =
      EnvelopeFuncs.MapEnvelope(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(MappingStates.Error);
    exception.ShouldBeSameAs(expectedException);

    capabilities.FromEnvelope.Received(1)(
      inputData.Envelope!,
      currentDate,
      InboxMessageStatus.Processing);
  }
}
