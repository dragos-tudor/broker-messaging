namespace Operations.Inbound.DeadLetter;

public partial class DeadLetterTests
{
  [TestMethod]
  [DataRow(true, "DeadLetterStates.InsertingSuccess")]
  [DataRow(false, "DeadLetterStates.InsertingIdempotent")]
  public async Task insert_dead_letter_message__persistence_result_varies__returns_matching_state(bool inserted, string expectedState)
  {
    var services = Substitute.For<IInsertingServices<string, string>>();
    var inputData = new DeadLetterData { DeadLetterMessage = Substitute.For<IDeadLetterMessage<string, string>>() };
    services.InsertDeadLetterMessageAsync(Arg.Any<IDeadLetterMessage<string, string>>(), Arg.Any<CancellationToken>()).Returns(inserted);

    var (data, state, exception) = await DeadLetterFuncs.InsertDeadLetterMessageAsync<IInsertingServices<string, string>, DeadLetterData, string, string>(services, inputData);

    data.ShouldBeSameAs(inputData);
    state.ShouldBe(expectedState);
    exception.ShouldBeNull();
  }

  [TestMethod]
  public async Task insert_dead_letter_message__persistence_throws__returns_error_with_exception()
  {
    var services = Substitute.For<IInsertingServices<string, string>>();
    var expectedException = new InvalidOperationException("insert failed");
    services.InsertDeadLetterMessageAsync(Arg.Any<IDeadLetterMessage<string, string>>(), Arg.Any<CancellationToken>()).ThrowsAsync(expectedException);
    var inputData = new DeadLetterData { DeadLetterMessage = Substitute.For<IDeadLetterMessage<string, string>>() };

    var (data, state, exception) = await DeadLetterFuncs.InsertDeadLetterMessageAsync<IInsertingServices<string, string>, DeadLetterData, string, string>(services, inputData);

    data.ShouldBeSameAs(inputData);
    state.ShouldBe(InsertingError);
    exception.ShouldBeSameAs(expectedException);
  }
}
