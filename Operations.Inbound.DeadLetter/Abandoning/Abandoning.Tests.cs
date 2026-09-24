namespace Operations.Inbound.DeadLetter;

public partial class DeadLetterTests
{
  [TestMethod]
  public async Task abandon_dead_letter_message__update_succeeds__returns_success()
  {
    var capabilities = Fixture.Create<AbandoningCapabilities<string, string>>();
    var message = Fixture.Create<IDeadLetterMessage<string, string>>();
    var inputData = new AbandoningData<string, string>(message);
    var expectedUpdate = new AbandoningUpdate(DeadLetterMessageStatus.Abandoned, message.LastError, null);
    capabilities.UpdateDeadLetterMessageAsync(inputData.DeadLetterMessage!, expectedUpdate, default).Returns(Task.CompletedTask);

    var (data, state, exception) = await DeadLetterFuncs.AbandonDeadLetterMessageAsync(capabilities, inputData, default);

    data.ShouldBe(inputData);
    state.ShouldBe(AbandoningStates.Success);
    exception.ShouldBeNull();
    capabilities.UpdateDeadLetterMessageAsync.Received(1)(inputData.DeadLetterMessage!, expectedUpdate, default);
  }

  [TestMethod]
  public async Task abandon_dead_letter_message__update_throws__returns_error_with_exception()
  {
    var capabilities = Fixture.Create<AbandoningCapabilities<string, string>>();
    var message = Fixture.Create<IDeadLetterMessage<string, string>>();
    var inputData = new AbandoningData<string, string>(message);
    var expectedUpdate = new AbandoningUpdate(DeadLetterMessageStatus.Abandoned, message.LastError, null);
    var expectedException = new InvalidOperationException("abandon failed");
    capabilities.UpdateDeadLetterMessageAsync(inputData.DeadLetterMessage!, expectedUpdate, default).ThrowsAsync(expectedException);

    var (data, state, exception) = await DeadLetterFuncs.AbandonDeadLetterMessageAsync(capabilities, inputData, default);

    data.ShouldBe(inputData);
    state.ShouldBe(AbandoningStates.Error);
    exception.ShouldBeSameAs(expectedException);
    capabilities.UpdateDeadLetterMessageAsync.Received(1)(inputData.DeadLetterMessage!, expectedUpdate, default);
  }
}
