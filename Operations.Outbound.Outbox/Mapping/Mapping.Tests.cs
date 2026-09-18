namespace Operations.Outbound.Outbox;

public partial class OutboxTests
{
  [TestMethod]
  public void map_outbox_message__mapper_returns_envelope__returns_success_and_sets_data()
  {
    var services = Substitute.For<IMappingServices<string, byte[], object, string, string>>();
    var message = new OutboxMessage<string, string> { MessageId = Guid.NewGuid(), MessageKey = "key", Payload = "payload", Type = "type", CreatedAt = DateTime.UtcNow };
    var envelope = Substitute.For<IEnvelope<string, byte[], object, string>>();
    services.FromOutboxMessage(message, message.CreatedAt).Returns(envelope);
    var inputData = new OutboxData { OutboxMessage = message };

    var (data, state, exception) = OutboxFuncs.MapOutboxMessage<IMappingServices<string, byte[], object, string, string>, OutboxData, string, byte[], object, string, string>(services, inputData);

    data.Envelope.ShouldBeSameAs(envelope);
    state.ShouldBe(MappingStates.Success);
    exception.ShouldBeNull();
  }

  [TestMethod]
  public void map_outbox_message__message_missing__returns_error()
  {
    var services = Substitute.For<IMappingServices<string, byte[], object, string, string>>();
    var inputData = new OutboxData();

    var (data, state, exception) = OutboxFuncs.MapOutboxMessage<IMappingServices<string, byte[], object, string, string>, OutboxData, string, byte[], object, string, string>(services, inputData);

    data.ShouldBeSameAs(inputData);
    state.ShouldBe(MappingStates.Error);
    exception.ShouldBeOfType<InvalidOperationException>();
  }

  [TestMethod]
  public void map_outbox_message__mapper_throws__returns_error_with_exception()
  {
    var services = Substitute.For<IMappingServices<string, byte[], object, string, string>>();
    var expectedException = new InvalidOperationException("mapping failed");
    services.FromOutboxMessage(Arg.Any<IOutboxMessage<string, string>>(), Arg.Any<DateTime>()).Throws(expectedException);
    var inputData = new OutboxData { OutboxMessage = Substitute.For<IOutboxMessage<string, string>>() };

    var (data, state, exception) = OutboxFuncs.MapOutboxMessage<IMappingServices<string, byte[], object, string, string>, OutboxData, string, byte[], object, string, string>(services, inputData);

    data.ShouldBeSameAs(inputData);
    state.ShouldBe(MappingStates.Error);
    exception.ShouldBeSameAs(expectedException);
  }
}
