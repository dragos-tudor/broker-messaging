namespace Operations.Inbound.Inbox;

public partial class InboxTests
{
  [TestMethod]
  [DataRow(true, InsertingStates.Success)]
  [DataRow(false, InsertingStates.Idempotent)]
  public async Task insert_inbox_message__persistence_result_varies__returns_matching_state(bool inserted, string expectedState)
  {
    var capabilities = Fixture.Create<InsertingCapabilities<string, string>>();
    var message = Fixture.Create<IInboxMessage<string, string>>();
    var inputData = CreateInboxData(message);
    capabilities.InsertInboxMessageAsync(GetInboxMessage<string, string>(inputData)!, default).Returns(inserted);

    var (data, state, exception) = await InboxFuncs.InsertInboxMessageAsync(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(expectedState);
    exception.ShouldBeNull();
    capabilities.InsertInboxMessageAsync.Received(1)(GetInboxMessage<string, string>(inputData)!, default);
  }

  [TestMethod]
  public async Task insert_inbox_message__message_missing__returns_error()
  {
    var capabilities = Fixture.Create<InsertingCapabilities<string, string>>();
    var inputData = CreateInboxData<string, string>();

    var (data, state, exception) = await InboxFuncs.InsertInboxMessageAsync(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(InsertingStates.Error);
    exception.ShouldBeOfType<InvalidOperationException>();
    capabilities.InsertInboxMessageAsync.Received(0)(default!, default);
  }
}
