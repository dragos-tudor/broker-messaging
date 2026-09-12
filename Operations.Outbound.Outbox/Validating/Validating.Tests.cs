namespace Operations.Outbound.Outbox;

public partial class OutboxTests
{
  [TestMethod]
  public async Task validate_outbox_message__message_is_valid__returns_success()
  {
    var services = Substitute.For<IValidatingServices<string, string>>();
    var message = new OutboxMessage<string, string> { MessageId = Guid.NewGuid(), MessageKey = "key", Payload = "payload", Type = "type", CreatedAt = DateTime.UtcNow };
    var inputData = new OutboxData { OutboxMessage = message };

    var (data, state, exception) = await OutboxFuncs.ValidateOutboxMessage<IValidatingServices<string, string>, OutboxData, string, string>(services, inputData);

    data.ShouldBeSameAs(inputData);
    state.ShouldBe(ValidatingSuccess);
    exception.ShouldBeNull();
  }

  [TestMethod]
  public async Task validate_outbox_message__message_missing__returns_error()
  {
    var services = Substitute.For<IValidatingServices<string, string>>();
    var inputData = new OutboxData();

    var (data, state, exception) = await OutboxFuncs.ValidateOutboxMessage<IValidatingServices<string, string>, OutboxData, string, string>(services, inputData);

    data.ShouldBeSameAs(inputData);
    state.ShouldBe(ValidatingError);
    exception.ShouldBeOfType<InvalidOperationException>();
  }
}
