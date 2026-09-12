namespace Operations.Outbound.Outbox;

public partial class OutboxTests
{
  [TestMethod]
  public async Task abandon_outbox_message__update_succeeds__returns_success()
  {
    var services = Substitute.For<IAbandoningServices<string, string>>();
    var inputData = new OutboxData { OutboxMessage = Substitute.For<IOutboxMessage<string, string>>() };
    services.UpdateOutboxMessageAsync(Arg.Any<IOutboxMessage<string, string>>(), Arg.Any<AbandoningUpdate>(), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

    var (data, state, exception) = await OutboxFuncs.AbandonOutboxMessageAsync<IAbandoningServices<string, string>, OutboxData, string, string>(services, inputData);

    data.ShouldBeSameAs(inputData);
    state.ShouldBe(AbandoningSuccess);
    exception.ShouldBeNull();
  }
}
