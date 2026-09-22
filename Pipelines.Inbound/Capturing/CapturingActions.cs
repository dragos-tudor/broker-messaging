
namespace Pipelines.Inbound;

internal enum CapturingActions
{
  None = 0,
  Capturing,
  Verifying,
  Mapping,
  Validating,
  Inserting,
  Confirming,
  ConfirmingFinal
}