namespace Operations.Outbound.Outbox;

public partial class OutboxTests
{
  [TestMethod]
  public void validate_outbox_message__message_is_valid__returns_success()
  {
    var capabilities = Fixture.Create<ValidatingCapabilities>();
    var message = Fixture.Create<IOutboxMessage>();
    var inputData = CreateOutboxData(message);

    capabilities.ValidateOutboxMessage(message).Returns(default(string));
    var (data, state, exception) = ValidateOutboxMessage(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(ValidatingStates.Success);
    exception.ShouldBeNull();
  }

  [TestMethod]
  public void validate_outbox_message__message_missing__returns_error()
  {
    var capabilities = Fixture.Create<ValidatingCapabilities>();
    var inputData = CreateOutboxData(default);

    var (data, state, exception) = ValidateOutboxMessage(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(ValidatingStates.Error);
    exception.ShouldBeOfType<InvalidOperationException>();
  }

  [TestMethod]
  public void validate_outbox_message__message_is_invalid__returns_invalid_error()
  {
    var capabilities = Fixture.Create<ValidatingCapabilities>();
    var message = Fixture.Create<IOutboxMessage>();
    var inputData = CreateOutboxData(message);

    capabilities.ValidateOutboxMessage(message).Returns("invalid");
    var (data, state, exception) = ValidateOutboxMessage(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(ValidatingStates.InvalidError);
    exception!.Message.ShouldBe("invalid");
  }
}
