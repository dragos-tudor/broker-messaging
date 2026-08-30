
namespace Persistence.InboxMessage;

static class InboxMessageConstraints
{
  internal const int PayloadMaxLength = 10_240_000;
  internal const int TypeMaxLength = 512;
  internal const int MetadataMaxLength = 2048;
  internal const int LastErrorMaxLength = 4096;
}