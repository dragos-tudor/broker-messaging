namespace Operations.Inbound.Inbox;

public partial class InboxTests
{
  sealed class ValidatingTestData : IValidatingData<string, string>
  {
    public InboxMessage<string, string>? InboxMessage { get; set; }
    public string? PipelineError { get; set; } = string.Empty;
  }

  [TestMethod]
  public async Task validating__validate_inbox_message__success_when_message_is_valid()
  {
    var services = Substitute.For<IValidatingServices>();
    var message = new InboxMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "valid-key",
      Payload = "valid-payload",
      CreatedAt = DateTime.UtcNow
    };

    var data = new ValidatingTestData { InboxMessage = message };

    var (resultData, state, exception) = await ValidateInboxMessage<IValidatingServices, ValidatingTestData, string, string>(services, data);

    state.ShouldBe(ValidatingSuccess);
    exception.ShouldBeNull();
    resultData.ShouldBeSameAs(data);
    resultData.InboxMessage.ShouldBeSameAs(message);
    resultData.PipelineError.ShouldBeEmpty();
  }

  [TestMethod]
  public async Task validating__validate_inbox_message__invalid_error_when_validation_fails()
  {
    var services = Substitute.For<IValidatingServices>();
    var message = new InboxMessage<string, string>
    {
      MessageId = Guid.Empty,
      MessageKey = "valid-key",
      Payload = "valid-payload",
      CreatedAt = DateTime.UtcNow
    };

    var data = new ValidatingTestData { InboxMessage = message };

    var (resultData, state, exception) = await ValidateInboxMessage<IValidatingServices, ValidatingTestData, string, string>(services, data);

    state.ShouldBe(ValidatingInvalidError);
    exception.ShouldNotBeNull();
    exception.Message.ShouldContain("MessageId is empty.");
    resultData.InboxMessage.ShouldBeNull();
    resultData.PipelineError?.ShouldContain("MessageId is empty.");
  }

  [TestMethod]
  public async Task validating__validate_inbox_message__error_when_message_null()
  {
    var services = Substitute.For<IValidatingServices>();
    var data = new ValidatingTestData { InboxMessage = null };

    var (resultData, state, exception) = await ValidateInboxMessage<IValidatingServices, ValidatingTestData, string, string>(services, data);

    state.ShouldBe(ValidatingError);
    exception.ShouldNotBeNull();
    exception.ShouldBeOfType<InvalidOperationException>();
    resultData.InboxMessage.ShouldBeNull();
    resultData.PipelineError.ShouldBe("Inbox message is required.");
  }
}
