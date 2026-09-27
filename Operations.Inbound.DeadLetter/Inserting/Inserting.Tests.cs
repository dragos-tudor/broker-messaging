namespace Operations.Inbound.DeadLetter;

public partial class DeadLetterTests
{
  [TestMethod]
  [DataRow(true, InsertingStates.Success)]
  [DataRow(false, InsertingStates.Idempotent)]
  public async Task insert_dead_letter_message__persistence_result_varies__returns_matching_state(bool inserted, string expectedState)
  {
    var capabilities = Fixture.Create<InsertingCapabilities<string, string>>();
    var message = Fixture.Create<IDeadLetterMessage<string, string>>();
    var inputData = CreateDeadLetterData(message);
    capabilities.InsertDeadLetterMessageAsync(GetDeadLetterMessage<string, string>(inputData)!, default).Returns(inserted);

    var (data, state, exception) = await DeadLetterFuncs.InsertDeadLetterMessageAsync(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(expectedState);
    exception.ShouldBeNull();
    capabilities.InsertDeadLetterMessageAsync.Received(1)(GetDeadLetterMessage<string, string>(inputData)!, default);
  }

  [TestMethod]
  public async Task insert_dead_letter_message__persistence_throws__returns_error_with_exception()
  {
    var capabilities = Fixture.Create<InsertingCapabilities<string, string>>();
    var message = Fixture.Create<IDeadLetterMessage<string, string>>();
    var inputData = CreateDeadLetterData(message);
    var expectedException = new InvalidOperationException("insert failed");
    capabilities.InsertDeadLetterMessageAsync(GetDeadLetterMessage<string, string>(inputData)!, default).ThrowsAsync(expectedException);

    var (data, state, exception) = await DeadLetterFuncs.InsertDeadLetterMessageAsync(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(InsertingStates.Error);
    exception.ShouldBeSameAs(expectedException);
    capabilities.InsertDeadLetterMessageAsync.Received(1)(GetDeadLetterMessage<string, string>(inputData)!, default);
  }
}
