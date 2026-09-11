
namespace Operations.Inbound.DeadLetterEnvelope;

partial class DeadLetterEnvelopeFuncs
{
  static ProduceResult CreateProduceResult(Guid messageId) =>
    new (){ MessageId = messageId };
}