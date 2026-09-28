namespace Operations.Outbound.Outbox;

public partial class OutboxTests
{
  [TestMethod]
  public void map_outbox_message__mapper_returns_envelope__returns_success_and_sets_data()
  {
    var capabilities = Fixture.Create<MappingCapabilities>();
    var message = Fixture.Create<IOutboxMessage>();
    var envelope = Fixture.Create<IEnvelope>();
    var inputData = CreateOutboxMappingData(message);
    capabilities.FromOutboxMessage(GetOutboxMessage(inputData)!, message.CreatedAt).Returns(envelope);

    var (data, state, exception) = MapOutboxMessage(capabilities, inputData);

    GetEnvelope(data).ShouldBeSameAs(envelope);
    state.ShouldBe(MappingStates.Success);
    exception.ShouldBeNull();
    capabilities.FromOutboxMessage.Received(1)(GetOutboxMessage(inputData)!, message.CreatedAt);
  }

  [TestMethod]
  public void map_outbox_message__message_missing__returns_error()
  {
    var capabilities = Fixture.Create<MappingCapabilities>();
    var inputData = CreateOutboxMappingData(default);

    var (data, state, exception) = MapOutboxMessage(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(MappingStates.Error);
    exception.ShouldBeOfType<InvalidOperationException>();
    capabilities.FromOutboxMessage.Received(0)(default!, default);
  }

  [TestMethod]
  public void map_outbox_message__mapper_throws__returns_error_with_exception()
  {
    var capabilities = Fixture.Create<MappingCapabilities>();
    var message = Fixture.Create<IOutboxMessage>();
    var inputData = CreateOutboxMappingData(message);
    var expectedException = new InvalidOperationException("mapping failed");
    capabilities.FromOutboxMessage(GetOutboxMessage(inputData)!, message.CreatedAt).Throws(expectedException);

    var (data, state, exception) = MapOutboxMessage(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(MappingStates.Error);
    exception.ShouldBeSameAs(expectedException);
    capabilities.FromOutboxMessage.Received(1)(GetOutboxMessage(inputData)!, message.CreatedAt);
  }
}
