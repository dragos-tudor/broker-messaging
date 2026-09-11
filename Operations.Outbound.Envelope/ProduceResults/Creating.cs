
namespace Operations.Outbound.Envelope;

partial class EnvelopeFuncs
{
  static ProduceResult CreateProduceResult(Guid messageId) =>
    new (){ MessageId = messageId };
}