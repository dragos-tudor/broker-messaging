
namespace Operations.Inbound.Envelope;

public partial class EnvelopeTests
{
  [TestMethod]
  public void map_envelope__mapper_returns_message__returns_success_and_sets_data()
  {
    object?[] inputData = [null, null, null, null, null];
    var capabilities = Fixture.Create<MappingCapabilities<string, byte[], object, string, string>>();
    var envelope = Fixture.Create<IEnvelope<string, byte[], object, string>>();
    var message = Fixture.Create<IInboxMessage<string, string>>();
    SetEnvelope(inputData, envelope);

    var currentDate = Fixture.Create<DateTime>();

    capabilities.GetUtcDateTime().Returns(currentDate);
    capabilities.FromEnvelope(envelope, currentDate, InboxMessageStatus.Processing)
      .Returns(message);

    var (data, state, exception) = MapEnvelope(capabilities, inputData);

    Console.WriteLine(inputData.Length);
    Console.WriteLine(inputData[2]);
    GetInboxMessage<string, string>(data).ShouldBe(message);
    state.ShouldBe(MappingStates.Success);
    exception.ShouldBeNull();

    capabilities.GetUtcDateTime.Received(1)();
    capabilities.FromEnvelope.Received(1)(
      GetEnvelope<string, byte[], object, string>(data)!,
      currentDate,
      InboxMessageStatus.Processing);
  }

  [TestMethod]
  public void map_envelope__envelope_missing__returns_error_with_exception()
  {
    object?[] inputData = [null, null, null, null, null];
    var capabilities = Fixture.Create<MappingCapabilities<string, byte[], object, string, string>>();

    var (data, state, exception) = MapEnvelope(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(MappingStates.Error);
    exception.ShouldBeOfType<InvalidOperationException>();

    capabilities.FromEnvelope.Received(0)(
      default!,
      default,
      InboxMessageStatus.Processing);
  }

  [TestMethod]
  public void map_envelope__mapper_throws__returns_error_with_exception()
  {
    object?[] inputData = [null, null, null, null, null];
    var capabilities = Fixture.Create<MappingCapabilities<string, byte[], object, string, string>>();
    var envelope = Fixture.Create<IEnvelope<string, byte[], object, string>>();
    SetEnvelope(inputData, envelope);

    var currentDate = Fixture.Create<DateTime>();
    var expectedException = new InvalidOperationException("mapping failed");

    capabilities.GetUtcDateTime().Returns(currentDate);
    capabilities.FromEnvelope(envelope, currentDate, InboxMessageStatus.Processing)
      .Throws(expectedException);

    var (data, state, exception) = MapEnvelope(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(MappingStates.Error);
    exception.ShouldBeSameAs(expectedException);

    capabilities.FromEnvelope.Received(1)(
      GetEnvelope<string, byte[], object, string>(data)!,
      currentDate,
      InboxMessageStatus.Processing);
  }
}
