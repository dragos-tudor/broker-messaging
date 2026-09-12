namespace Operations.Inbound.DeadLetterEnvelope;

public partial class DeadLetterEnvelopeTests
{
  [TestMethod]
  public async Task publish_dead_letter_envelope__publisher_succeeds__returns_success()
  {
    var services = Substitute.For<IPublishingServices<string, byte[], object, string, string>>();
    var envelope = Substitute.For<IDeadLetterEnvelope<string, byte[], object, string>>();
    var inputData = new DeadLetterEnvelopeData { DeadLetterEnvelope = envelope };

    var (data, state, exception) = await DeadLetterEnvelopeFuncs.PublishDeadLetterEnvelopeAsync<IPublishingServices<string, byte[], object, string, string>, DeadLetterEnvelopeData, string, byte[], object, string, string>(services, inputData);

    data.ShouldBeSameAs(inputData);
    state.ShouldBe(PublishingSuccess);
    exception.ShouldBeNull();
  }

  [TestMethod]
  public async Task publish_dead_letter_envelope__envelope_missing__returns_error_with_exception()
  {
    var services = Substitute.For<IPublishingServices<string, byte[], object, string, string>>();
    var inputData = new DeadLetterEnvelopeData();

    var (data, state, exception) = await DeadLetterEnvelopeFuncs.PublishDeadLetterEnvelopeAsync<IPublishingServices<string, byte[], object, string, string>, DeadLetterEnvelopeData, string, byte[], object, string, string>(services, inputData);

    data.ShouldBeSameAs(inputData);
    state.ShouldBe(PublishingError);
    exception.ShouldBeOfType<InvalidOperationException>();
  }
}
