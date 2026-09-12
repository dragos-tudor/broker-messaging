namespace Operations.Outbound.Outbox;

public partial class OutboxTests
{
  [TestMethod]
  public async Task close_outbox_message__update_succeeds__returns_success()
  {
    var services = Substitute.For<IClosingServices<string, string>>();
    var inputData = new OutboxData { OutboxMessage = Substitute.For<IOutboxMessage<string, string>>() };
    services.UpdateOutboxMessageAsync(Arg.Any<IOutboxMessage<string, string>>(), Arg.Any<ClosingUpdate>(), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

    var (data, state, exception) = await OutboxFuncs.CloseOutboxMessageAsync<IClosingServices<string, string>, OutboxData, string, string>(services, inputData);

    data.ShouldBeSameAs(inputData);
    state.ShouldBe(ClosingSuccess);
    exception.ShouldBeNull();
  }
}
