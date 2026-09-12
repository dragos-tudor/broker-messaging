namespace Operations.Outbound.Envelope;

public partial class EnvelopeTests
{
  [TestMethod]
  [DataRow(true, "Envelope.DispatchingAck")]
  [DataRow(false, "Envelope.DispatchingNotAck")]
  public async Task dispatch_envelope__produce_result_acknowledgement_varies__returns_matching_state(
    bool isAcknowledged,
    string expectedState)
  {
    var services = Substitute.For<IDispatchingServices>();
    var inputData = new EnvelopeData {
      ProduceResult = new ProduceResult { IsAcknowledged = isAcknowledged }
    };

    var (data, state, exception) = await EnvelopeFuncs.DispatchEnvelope<
      IDispatchingServices, EnvelopeData>(services, inputData);

    data.ShouldBeSameAs(inputData);
    state.ShouldBe(expectedState);
    exception.ShouldBeNull();
  }

  [TestMethod]
  public async Task dispatch_envelope__produce_result_missing__returns_error_with_exception()
  {
    var services = Substitute.For<IDispatchingServices>();
    var inputData = new EnvelopeData();

    var (data, state, exception) = await EnvelopeFuncs.DispatchEnvelope<
      IDispatchingServices, EnvelopeData>(services, inputData);

    data.ShouldBeSameAs(inputData);
    state.ShouldBe(DispatchingError);
    exception.ShouldBeOfType<InvalidOperationException>();
  }
}
