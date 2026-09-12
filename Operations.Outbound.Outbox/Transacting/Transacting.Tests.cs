namespace Operations.Outbound.Outbox;

public partial class OutboxTests
{
  [TestMethod]
  public async Task transact_outbox_message__domain_model_missing__returns_error()
  {
    var services = Substitute.For<ITransactingServices<string, string, IDisposable>>();
    var inputData = new OutboxData { OutboxMessage = Substitute.For<IOutboxMessage<string, string>>() };

    var (data, state, exception) = await OutboxFuncs.TransactOutboxMessageAsync<ITransactingServices<string, string, IDisposable>, OutboxData, string, string, IDisposable>(services, inputData);

    data.ShouldBeSameAs(inputData);
    state.ShouldBe(TransactingError);
    exception.ShouldBeOfType<InvalidOperationException>();
  }
}
