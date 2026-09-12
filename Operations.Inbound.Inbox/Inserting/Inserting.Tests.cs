namespace Operations.Inbound.Inbox;

public partial class InboxTests
{
  [TestMethod]
  [DataRow(true, "InboxStates.InsertingSuccess")]
  [DataRow(false, "InboxStates.InsertingIdempotent")]
  public async Task insert_inbox_message__persistence_result_varies__returns_matching_state(bool inserted, string expectedState)
  {
    var services = Substitute.For<IInsertingServices<string, string>>();
    var inputData = new InboxData { InboxMessage = InboxData.CreateMessage() };
    services.InsertInboxMessageAsync(Arg.Any<IInboxMessage<string, string>>(), Arg.Any<CancellationToken>()).Returns(inserted);

    var (data, state, exception) = await InboxFuncs.InsertInboxMessageAsync<IInsertingServices<string, string>, InboxData, string, string>(services, inputData);

    data.ShouldBeSameAs(inputData);
    state.ShouldBe(expectedState);
    exception.ShouldBeNull();
  }

  [TestMethod]
  public async Task insert_inbox_message__message_missing__returns_error()
  {
    var services = Substitute.For<IInsertingServices<string, string>>();
    var inputData = new InboxData();

    var (data, state, exception) = await InboxFuncs.InsertInboxMessageAsync<IInsertingServices<string, string>, InboxData, string, string>(services, inputData);

    data.ShouldBeSameAs(inputData);
    state.ShouldBe(InsertingError);
    exception.ShouldBeOfType<InvalidOperationException>();
  }
}
