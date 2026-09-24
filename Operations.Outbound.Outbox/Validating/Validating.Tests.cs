namespace Operations.Outbound.Outbox;

public partial class OutboxTests
{
  [TestMethod]
  public void validate_outbox_message__message_is_valid__returns_success()
  {
    var capabilities = Fixture.Create<ValidatingCapabilities>();
    var message = Fixture.Create<IOutboxMessage<string, string>>();
    var inputData = new ValidatingData<string, string>(message);

    var (data, state, exception) = OutboxFuncs.ValidateOutboxMessage(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(ValidatingStates.Success);
    exception.ShouldBeNull();
  }

  [TestMethod]
  public void validate_outbox_message__message_missing__returns_error()
  {
    var capabilities = Fixture.Create<ValidatingCapabilities>();
    var inputData = new ValidatingData<string, string>(default);

    var (data, state, exception) = OutboxFuncs.ValidateOutboxMessage(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(ValidatingStates.Error);
    exception.ShouldBeOfType<InvalidOperationException>();
  }

  [TestMethod]
  public void validate_outbox_message__message_is_invalid__returns_invalid_error()
  {
    var capabilities = Fixture.Create<ValidatingCapabilities>();
    var message = Fixture.Build<OutboxMessage<string, string>>()
      .With(message => message.MessageId, Guid.Empty)
      .Create();
    var inputData = new ValidatingData<string, string>(message);

    var (data, state, exception) = OutboxFuncs.ValidateOutboxMessage(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(ValidatingStates.InvalidError);
    exception.ShouldNotBeNull();
  }
}
