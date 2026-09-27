namespace Operations.Inbound.DeadLetter;

public partial class DeadLetterTests
{
  [TestMethod]
  public async Task close_dead_letter_message__update_succeeds__returns_success()
  {
    var capabilities = Fixture.Create<ClosingCapabilities<string, string>>();
    var message = Fixture.Create<IDeadLetterMessage<string, string>>();
    var inputData = CreateDeadLetterData(message);
    var expectedUpdate = new ClosingUpdate(DeadLetterMessageStatus.Published);
    capabilities.UpdateDeadLetterMessageAsync(GetDeadLetterMessage<string, string>(inputData)!, expectedUpdate, default).Returns(Task.CompletedTask);

    var (data, state, exception) = await DeadLetterFuncs.CloseDeadLetterMessageAsync(capabilities, inputData, default);

    data.ShouldBe(inputData);
    state.ShouldBe(ClosingStates.Success);
    exception.ShouldBeNull();
    capabilities.UpdateDeadLetterMessageAsync.Received(1)(GetDeadLetterMessage<string, string>(inputData)!, expectedUpdate, default);
  }

  [TestMethod]
  public async Task close_dead_letter_message__update_throws__returns_error_with_exception()
  {
    var capabilities = Fixture.Create<ClosingCapabilities<string, string>>();
    var message = Fixture.Create<IDeadLetterMessage<string, string>>();
    var inputData = CreateDeadLetterData(message);
    var expectedUpdate = new ClosingUpdate(DeadLetterMessageStatus.Published);
    var expectedException = new InvalidOperationException("close failed");
    capabilities.UpdateDeadLetterMessageAsync(GetDeadLetterMessage<string, string>(inputData)!, expectedUpdate, default).ThrowsAsync(expectedException);

    var (data, state, exception) = await DeadLetterFuncs.CloseDeadLetterMessageAsync(capabilities, inputData, default);

    data.ShouldBe(inputData);
    state.ShouldBe(ClosingStates.Error);
    exception.ShouldBeSameAs(expectedException);
    capabilities.UpdateDeadLetterMessageAsync.Received(1)(GetDeadLetterMessage<string, string>(inputData)!, expectedUpdate, default);
  }
}
