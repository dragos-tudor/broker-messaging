namespace Operations.Outbound.Outbox;

public partial class OutboxTests
{
  [TestMethod]
  public async Task close_outbox_message__update_succeeds__returns_success()
  {
    var capabilities = Fixture.Create<ClosingCapabilities<string, string>>();
    var message = Fixture.Create<IOutboxMessage<string, string>>();
    var inputData = CreateOutboxData(message);
    var expectedUpdate = new ClosingUpdate(OutboxMessageStatus.Published);
    capabilities.UpdateOutboxMessageAsync(GetOutboxMessage<string, string>(inputData)!, expectedUpdate, default).Returns(Task.CompletedTask);

    var (data, state, exception) = await CloseOutboxMessageAsync(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(ClosingStates.Success);
    exception.ShouldBeNull();
    capabilities.UpdateOutboxMessageAsync.Received(1)(GetOutboxMessage<string, string>(inputData)!, expectedUpdate, default);
  }

  [TestMethod]
  public async Task close_outbox_message__update_throws__returns_error_with_exception()
  {
    var capabilities = Fixture.Create<ClosingCapabilities<string, string>>();
    var message = Fixture.Create<IOutboxMessage<string, string>>();
    var inputData = CreateOutboxData(message);
    var expectedUpdate = new ClosingUpdate(OutboxMessageStatus.Published);
    var expectedException = new InvalidOperationException("close failed");
    capabilities.UpdateOutboxMessageAsync(GetOutboxMessage<string, string>(inputData)!, expectedUpdate, default).ThrowsAsync(expectedException);

    var (data, state, exception) = await CloseOutboxMessageAsync(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(ClosingStates.Error);
    exception.ShouldBeSameAs(expectedException);
    capabilities.UpdateOutboxMessageAsync.Received(1)(GetOutboxMessage<string, string>(inputData)!, expectedUpdate, default);
  }
}
