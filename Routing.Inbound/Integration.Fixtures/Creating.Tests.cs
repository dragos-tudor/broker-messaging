using Services = Routing.Inbound.IInboundRoutingServices<string, int, string, string, byte[], System.IDisposable>;
using Data = Routing.Inbound.IInboundRoutingData<string, int, string, string, byte[]>;
using Persistence.InboxMessage;
using Persistence.DeadLetterMessage;
using Transport.DeadLetterEnvelope;
using Transport.Envelope;

namespace Routing.Inbound;

partial class IntegrationTests
{
  static Data CreateData()
  {
    var data = Substitute.For<Data>();
    data.DeadLetterEnvelope = default;
    data.DeadLetterMessage = default;
    data.Envelope = default;
    data.InboxMessage = default;
    data.DomainModel = default;
    data.ProduceResult = default;
    return data;
  }

  static Services CreateServices(
    List<(Enum, Enum)> pipelineLogs,
    List<(string, Enum, string?)> operationLogs,
    FastRetryOptions? fastRetryOptions = null,
    CancellationToken ct = default)
  {
    var services = Substitute.For<Services>();

    ConfigureInboundPipelineConfig(services);
    ConfigureFastRetryOptions(services, fastRetryOptions);

    ConfigurePipelineInstrumentations(pipelineLogs, services);
    ConfigureOperationsInstrumentations(operationLogs, services);

    return services;
  }

  static DeadLetterMessage<string, byte[]> CreateDeadletterMessage() => Fixture.Create<DeadLetterMessage<string, byte[]>>();

  static IDeadLetterEnvelope<string, int, string, string> CreateDeadletterEnvelope() => Fixture.Create<IDeadLetterEnvelope<string, int, string, string>>();

  static TModel CreateDomainModel<TModel>() => Fixture.Create<TModel>();

  static IEnvelope<string, int, string, string> CreateEnvelope() => Fixture.Create<IEnvelope<string, int, string, string>>();

  static IInboxMessage<string, byte[]> CreateInboxMessage()
  {
    var message = Fixture.Create<IInboxMessage<string, byte[]>>();
    message.RetryCount = 0;
    return message;
  }

  static List<(string, Enum, string?)> CreateOperationLogs() => [];

  static List<(Enum, Enum)> CreatePipelineLogs() => [];

  static ProduceResult CreateProduceResult() => Fixture.Create<ProduceResult>();
}
