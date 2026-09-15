
namespace Operations.Inbound.Envelope;

public partial class EnvelopeTests
{
  [TestMethod]
  public async Task map_envelope__mapper_returns_message__returns_success_and_sets_data()
  {
    var services = Substitute.For<IMappingServices<string, byte[], object, string, string>>();
    var envelope = Substitute.For<IEnvelope<string, byte[], object, string>>();
    var message = new InboxMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      TransportMessageId = "transport",
      MessageKey = "key",
      Payload = "payload",
      CreatedAt = DateTime.UtcNow,
      Type = "type"
    };
    services.GetUtcDateTime().Returns(DateTime.UtcNow);
    services.FromEnvelope(envelope, Arg.Any<DateTime>()).Returns(message);
    var inputData = new EnvelopeData { Envelope = envelope };

    var (data, state, exception) = await EnvelopeFuncs.MapEnvelope<IMappingServices<string, byte[], object, string, string>, EnvelopeData, string, byte[], object, string, string>(services, inputData);

    data.ShouldBeSameAs(inputData);
    data.InboxMessage.ShouldBeSameAs(message);
    state.ShouldBe(MappingStates.Success);
    exception.ShouldBeNull();
  }

  [TestMethod]
  public async Task map_envelope__envelope_missing__returns_error_with_exception()
  {
    var services = Substitute.For<IMappingServices<string, byte[], object, string, string>>();
    var inputData = new EnvelopeData();

    var (data, state, exception) = await EnvelopeFuncs.MapEnvelope<IMappingServices<string, byte[], object, string, string>, EnvelopeData, string, byte[], object, string, string>(services, inputData);

    data.ShouldBeSameAs(inputData);
    state.ShouldBe(MappingStates.Error);
    exception.ShouldBeOfType<InvalidOperationException>();
  }

  [TestMethod]
  public async Task map_envelope__mapper_throws__returns_error_with_exception()
  {
    var services = Substitute.For<IMappingServices<string, byte[], object, string, string>>();
    var expectedException = new InvalidOperationException("mapping failed");
    services.FromEnvelope(Arg.Any<IEnvelope<string, byte[], object, string>>(), Arg.Any<DateTime>()).Throws(expectedException);
    var inputData = new EnvelopeData { Envelope = Substitute.For<IEnvelope<string, byte[], object, string>>() };

    var (data, state, exception) = await EnvelopeFuncs.MapEnvelope<IMappingServices<string, byte[], object, string, string>, EnvelopeData, string, byte[], object, string, string>(services, inputData);

    data.ShouldBeSameAs(inputData);
    state.ShouldBe(MappingStates.Error);
    exception.ShouldBeSameAs(expectedException);
  }
}
