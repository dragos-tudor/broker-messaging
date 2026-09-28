namespace Operations.Inbound.DeadLetter;

public partial class DeadLetterTests
{
  [TestMethod]
  public void map_dead_letter_message__mapper_returns_envelope__returns_success_and_sets_data()
  {
    var capabilities = Fixture.Create<MappingCapabilities>();
    var message = Fixture.Create<IDeadLetterMessage>();
    var envelope = Fixture.Create<IDeadLetterEnvelope<string, byte[], object, string>>();
    var inputData = CreateDeadLetterData(message);
    capabilities.FromDeadLetterMessage(GetDeadLetterMessage(inputData)!, message.OriginatedAt).Returns(envelope);

    var (data, state, exception) = DeadLetterFuncs.MapDeadLetterMessage(capabilities, inputData);

    GetDeadLetterEnvelope<string, byte[], object, string>(data).ShouldBeSameAs(envelope);
    capabilities.FromDeadLetterMessage.Received(1)(GetDeadLetterMessage(inputData)!, message.OriginatedAt);
    state.ShouldBe(MappingStates.Success);
    exception.ShouldBeNull();
  }

  [TestMethod]
  public void map_dead_letter_message__message_missing__returns_error()
  {
    var capabilities = Fixture.Create<MappingCapabilities>();
    var inputData = CreateDeadLetterData(default);

    var (data, state, exception) = DeadLetterFuncs.MapDeadLetterMessage(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(MappingStates.Error);
    exception.ShouldBeOfType<InvalidOperationException>();
    capabilities.FromDeadLetterMessage.Received(0)(default!, default);
  }

  [TestMethod]
  public void map_dead_letter_message__mapper_throws__returns_error_with_exception()
  {
    var capabilities = Fixture.Create<MappingCapabilities>();
    var message = Fixture.Create<IDeadLetterMessage>();
    var inputData = CreateDeadLetterData(message);
    var expectedException = new InvalidOperationException("mapping failed");
    capabilities.FromDeadLetterMessage(GetDeadLetterMessage(inputData)!, message.OriginatedAt).Throws(expectedException);

    var (data, state, exception) = DeadLetterFuncs.MapDeadLetterMessage(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(MappingStates.Error);
    exception.ShouldBeSameAs(expectedException);
    capabilities.FromDeadLetterMessage.Received(1)(GetDeadLetterMessage(inputData)!, message.OriginatedAt);
  }
}
