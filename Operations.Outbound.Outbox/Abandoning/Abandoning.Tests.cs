namespace Operations.Outbound.Outbox;

public partial class OutboxTests
{
  [TestMethod]
  public async Task abandon_outbox_message__update_succeeds__returns_success()
  {
    var capabilities = Fixture.Create<AbandoningCapabilities<string, string>>();
    var message = Fixture.Create<IOutboxMessage<string, string>>();
    var inputData = new AbandoningData<string, string>(message);
    var expectedUpdate = new AbandoningUpdate(OutboxMessageStatus.Abandoned, message.LastError);
    capabilities.UpdateOutboxMessageAsync(inputData.OutboxMessage!, expectedUpdate, default).Returns(Task.CompletedTask);

    var (data, state, exception) = await OutboxFuncs.AbandonOutboxMessageAsync(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(AbandoningStates.Success);
    exception.ShouldBeNull();
    capabilities.UpdateOutboxMessageAsync.Received(1)(inputData.OutboxMessage!, expectedUpdate, default);
  }

  [TestMethod]
  public async Task abandon_outbox_message__update_throws__returns_error_with_exception()
  {
    var capabilities = Fixture.Create<AbandoningCapabilities<string, string>>();
    var message = Fixture.Create<IOutboxMessage<string, string>>();
    var inputData = new AbandoningData<string, string>(message);
    var expectedUpdate = new AbandoningUpdate(OutboxMessageStatus.Abandoned, message.LastError);
    var expectedException = new InvalidOperationException("abandon failed");
    capabilities.UpdateOutboxMessageAsync(inputData.OutboxMessage!, expectedUpdate, default).ThrowsAsync(expectedException);

    var (data, state, exception) = await OutboxFuncs.AbandonOutboxMessageAsync(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(AbandoningStates.Error);
    exception.ShouldBeSameAs(expectedException);
    capabilities.UpdateOutboxMessageAsync.Received(1)(inputData.OutboxMessage!, expectedUpdate, default);
  }
}
