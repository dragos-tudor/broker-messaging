namespace Operations.Inbound.DeadLetter;

public partial class DeadLetterTests
{
  [TestMethod]
  public async Task map_dead_letter_message__mapper_returns_envelope__returns_success_and_sets_data()
  {
    var services = Substitute.For<IMappingServices<string, byte[], object, string, string>>();
    var message = Substitute.For<IDeadLetterMessage<string, string>>();
    var envelope = Substitute.For<IDeadLetterEnvelope<string, byte[], object, string>>();
    services.FromDeadLetterMessage(message, Arg.Any<DateTime>()).Returns(envelope);
    var inputData = new DeadLetterData { DeadLetterMessage = message };

    var (data, state, exception) = await DeadLetterFuncs.MapDeadLetterMessage<IMappingServices<string, byte[], object, string, string>, DeadLetterData, string, byte[], object, string, string>(services, inputData);

    data.DeadLetterEnvelope.ShouldBeSameAs(envelope);
    state.ShouldBe(MappingSuccess);
    exception.ShouldBeNull();
  }

  [TestMethod]
  public async Task map_dead_letter_message__message_missing__returns_error()
  {
    var services = Substitute.For<IMappingServices<string, byte[], object, string, string>>();
    var inputData = new DeadLetterData();

    var (data, state, exception) = await DeadLetterFuncs.MapDeadLetterMessage<IMappingServices<string, byte[], object, string, string>, DeadLetterData, string, byte[], object, string, string>(services, inputData);

    data.ShouldBeSameAs(inputData);
    state.ShouldBe(MappingError);
    exception.ShouldBeOfType<InvalidOperationException>();
  }
}
