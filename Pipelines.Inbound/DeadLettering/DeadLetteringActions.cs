
namespace Pipelines.Inbound;

internal enum DeadLetteringActions
{
  Converting,
  Inserting,
  Abandoning,
  Closing
}
