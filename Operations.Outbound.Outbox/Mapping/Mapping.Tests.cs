namespace Operations.Outbound.Outbox;

public partial class OutboxTests
{
  [TestMethod]
  public void map_outbox_message__mapper_returns_envelope__returns_success_and_sets_data()
  {
    var capabilities = Fixture.Create<MappingCapabilities<string, byte[], object, string, string>>();
    var message = Fixture.Create<IOutboxMessage<string, string>>();
    var envelope = Fixture.Create<IEnvelope<string, byte[], object, string>>();
    var inputData = new MappingData<string, byte[], object, string, string>(message, default);
    capabilities.FromOutboxMessage(inputData.OutboxMessage!, message.CreatedAt).Returns(envelope);

    var (data, state, exception) = OutboxFuncs.MapOutboxMessage(capabilities, inputData);

    data.Envelope.ShouldBeSameAs(envelope);
    state.ShouldBe(MappingStates.Success);
    exception.ShouldBeNull();
    capabilities.FromOutboxMessage.Received(1)(inputData.OutboxMessage!, message.CreatedAt);
  }

  [TestMethod]
  public void map_outbox_message__message_missing__returns_error()
  {
    var capabilities = Fixture.Create<MappingCapabilities<string, byte[], object, string, string>>();
    var inputData = new MappingData<string, byte[], object, string, string>(default, default);

    var (data, state, exception) = OutboxFuncs.MapOutboxMessage(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(MappingStates.Error);
    exception.ShouldBeOfType<InvalidOperationException>();
    capabilities.FromOutboxMessage.Received(0)(default!, default);
  }

  [TestMethod]
  public void map_outbox_message__mapper_throws__returns_error_with_exception()
  {
    var capabilities = Fixture.Create<MappingCapabilities<string, byte[], object, string, string>>();
    var message = Fixture.Create<IOutboxMessage<string, string>>();
    var inputData = new MappingData<string, byte[], object, string, string>(message, default);
    var expectedException = new InvalidOperationException("mapping failed");
    capabilities.FromOutboxMessage(inputData.OutboxMessage!, message.CreatedAt).Throws(expectedException);

    var (data, state, exception) = OutboxFuncs.MapOutboxMessage(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(MappingStates.Error);
    exception.ShouldBeSameAs(expectedException);
    capabilities.FromOutboxMessage.Received(1)(inputData.OutboxMessage!, message.CreatedAt);
  }
}
