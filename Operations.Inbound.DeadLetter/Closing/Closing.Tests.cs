namespace Operations.Inbound.DeadLetter;

public partial class DeadLetterTests
{
  [TestMethod]
  public async Task close_dead_letter_message__update_succeeds__returns_success()
  {
    var services = Substitute.For<IClosingServices<string, string>>();
    var inputData = new DeadLetterData { DeadLetterMessage = Substitute.For<IDeadLetterMessage<string, string>>() };
    services.UpdateDeadLetterMessageAsync(Arg.Any<IDeadLetterMessage<string, string>>(), Arg.Any<ClosingUpdate>(), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

    var (data, state, exception) = await DeadLetterFuncs.CloseDeadLetterMessageAsync<IClosingServices<string, string>, DeadLetterData, string, string>(services, inputData, default);

    data.ShouldBeSameAs(inputData);
    state.ShouldBe(ClosingSuccess);
    exception.ShouldBeNull();
  }

  [TestMethod]
  public async Task close_dead_letter_message__update_throws__returns_error_with_exception()
  {
    var services = Substitute.For<IClosingServices<string, string>>();
    var expectedException = new InvalidOperationException("close failed");
    var inputData = new DeadLetterData { DeadLetterMessage = Substitute.For<IDeadLetterMessage<string, string>>() };
    services.UpdateDeadLetterMessageAsync(Arg.Any<IDeadLetterMessage<string, string>>(), Arg.Any<ClosingUpdate>(), Arg.Any<CancellationToken>()).ThrowsAsync(expectedException);

    var (data, state, exception) = await DeadLetterFuncs.CloseDeadLetterMessageAsync<IClosingServices<string, string>, DeadLetterData, string, string>(services, inputData, default);

    data.ShouldBeSameAs(inputData);
    state.ShouldBe(ClosingError);
    exception.ShouldBeSameAs(expectedException);
  }
}
