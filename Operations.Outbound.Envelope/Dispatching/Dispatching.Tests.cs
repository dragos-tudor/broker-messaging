namespace Operations.Outbound.Envelope;

public partial class EnvelopeTests
{
  [TestMethod]
  [DataRow(true, DispatchingStates.Ack)]
  [DataRow(false, DispatchingStates.NotAck)]
  public void dispatch_envelope__produce_result_acknowledgement_varies__returns_matching_state(
    bool acknowledged,
    string expectedState)
  {
    var capabilities = Fixture.Create<DispatchingCapabilities>();
    var inputData = new DispatchingData(
      new ProduceResult { IsAcknowledged = acknowledged });

    var (data, state, exception) =
      EnvelopeFuncs.DispatchEnvelope(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(expectedState);
    exception.ShouldBeNull();
  }

  [TestMethod]
  public void dispatch_envelope__produce_result_missing__returns_error_with_exception()
  {
    var capabilities = Fixture.Create<DispatchingCapabilities>();
    var inputData = new DispatchingData(null);

    var (data, state, exception) =
      EnvelopeFuncs.DispatchEnvelope(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(DispatchingStates.Error);
    exception.ShouldBeOfType<InvalidOperationException>();
  }
}
