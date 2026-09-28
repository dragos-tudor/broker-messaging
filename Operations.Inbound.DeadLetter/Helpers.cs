
namespace Operations.Inbound.DeadLetter;

partial class DeadLetterFuncs
{
  static IDeadLetterMessage RequireDeadLetterMessage(
    IDeadLetterMessage? message) =>
    message ?? throw new InvalidOperationException("Dead letter message is required.");
}
