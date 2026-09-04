namespace Operations.Outbound.Outbox;

public partial class OutboxTests
{
  sealed class ValidatingTestData : IValidatingData<string, string>
  {
    public OutboxMessage<string, string>? OutboxMessage { get; set; }
  }

  [TestMethod]
  public async Task validating__validate_outbox_message__success_when_message_is_valid()
  {
    var services = Substitute.For<IValidatingServices<string, string>>();
    var message = new OutboxMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "valid-outbox-key",
      Payload = "valid-payload",
      CreatedAt = DateTime.UtcNow
    };

    var data = new ValidatingTestData { OutboxMessage = message };

    var (resultData, state, exception) = await ValidateOutboxMessage<IValidatingServices<string, string>, ValidatingTestData, string, string>(services, data);

    state.ShouldBe(ValidatingSuccess);
    exception.ShouldBeNull();
    resultData.ShouldBeSameAs(data);
    resultData.OutboxMessage.ShouldBeSameAs(message);
  }

  [TestMethod]
  public async Task validating__validate_outbox_message__invalid_error_when_validation_fails()
  {
    var services = Substitute.For<IValidatingServices<string, string>>();
    var message = new OutboxMessage<string, string>
    {
      MessageId = Guid.Empty, // Invalid empty Guid
      MessageKey = "invalid-key",
      Payload = "payload",
      CreatedAt = DateTime.UtcNow
    };

    var data = new ValidatingTestData { OutboxMessage = message };

    var (resultData, state, exception) = await ValidateOutboxMessage<IValidatingServices<string, string>, ValidatingTestData, string, string>(services, data);

    state.ShouldBe(ValidatingInvalidError);
    exception.ShouldNotBeNull();
    exception.Message.ShouldContain("MessageId is empty.");
    resultData.OutboxMessage.ShouldBeNull();
  }

  [TestMethod]
  public async Task validating__validate_outbox_message__error_when_message_null()
  {
    var services = Substitute.For<IValidatingServices<string, string>>();
    var data = new ValidatingTestData { OutboxMessage = null };

    var (resultData, state, exception) = await ValidateOutboxMessage<IValidatingServices<string, string>, ValidatingTestData, string, string>(services, data);

    state.ShouldBe(ValidatingError);
    exception.ShouldNotBeNull();
    exception.ShouldBeOfType<InvalidOperationException>();
    exception.Message.ShouldBe("Outbox message is required.");
    resultData.OutboxMessage.ShouldBeNull();
  }
}
