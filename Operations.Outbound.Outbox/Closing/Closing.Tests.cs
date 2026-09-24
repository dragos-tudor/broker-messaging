namespace Operations.Outbound.Outbox;

public partial class OutboxTests
{
  [TestMethod]
  public async Task close_outbox_message__update_succeeds__returns_success()
  {
    var capabilities = Fixture.Create<ClosingCapabilities<string, string>>();
    var message = Fixture.Create<IOutboxMessage<string, string>>();
    var inputData = new ClosingData<string, string>(message);
    var expectedUpdate = new ClosingUpdate(OutboxMessageStatus.Published);
    capabilities.UpdateOutboxMessageAsync(inputData.OutboxMessage!, expectedUpdate, default).Returns(Task.CompletedTask);

    var (data, state, exception) = await OutboxFuncs.CloseOutboxMessageAsync(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(ClosingStates.Success);
    exception.ShouldBeNull();
    capabilities.UpdateOutboxMessageAsync.Received(1)(inputData.OutboxMessage!, expectedUpdate, default);
  }

  [TestMethod]
  public async Task close_outbox_message__update_throws__returns_error_with_exception()
  {
    var capabilities = Fixture.Create<ClosingCapabilities<string, string>>();
    var message = Fixture.Create<IOutboxMessage<string, string>>();
    var inputData = new ClosingData<string, string>(message);
    var expectedUpdate = new ClosingUpdate(OutboxMessageStatus.Published);
    var expectedException = new InvalidOperationException("close failed");
    capabilities.UpdateOutboxMessageAsync(inputData.OutboxMessage!, expectedUpdate, default).ThrowsAsync(expectedException);

    var (data, state, exception) = await OutboxFuncs.CloseOutboxMessageAsync(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(ClosingStates.Error);
    exception.ShouldBeSameAs(expectedException);
    capabilities.UpdateOutboxMessageAsync.Received(1)(inputData.OutboxMessage!, expectedUpdate, default);
  }
}
