namespace Operations.Inbound.DeadLetter;

public partial class DeadLetterTests
{
  [TestMethod]
  public async Task abandon_dead_letter_message__update_succeeds__returns_success()
  {
    var services = Substitute.For<IAbandoningServices<string, string>>();
    var inputData = new DeadLetterData { DeadLetterMessage = Substitute.For<IDeadLetterMessage<string, string>>() };
    services.UpdateDeadLetterMessageAsync(Arg.Any<IDeadLetterMessage<string, string>>(), Arg.Any<AbandoningUpdate>(), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

    var (data, state, exception) = await DeadLetterFuncs.AbandonDeadLetterMessageAsync<IAbandoningServices<string, string>, DeadLetterData, string, string>(services, inputData, default);

    data.ShouldBeSameAs(inputData);
    state.ShouldBe(AbandoningSuccess);
    exception.ShouldBeNull();
  }
}
