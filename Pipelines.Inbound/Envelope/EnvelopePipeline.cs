
using static Operations.Inbound.Envelope.EnvelopeStates;

namespace Pipelines.Inbound;

partial class InboundFuncs
{
  internal static EnvelopeOperation EnvelopePipeline(string state) => state switch
  {
      // Capturing
      NotCapturedEnvelopeState => EnvelopeOperation.Capturing,        // self-loop, nothing polled
      CaptureEnvelopeErrorState => EnvelopeOperation.Capturing,       // self-loop, technical
      CaptureEnvelopeSuccessState => EnvelopeOperation.Validating,

      // Validating
      ValidateEnvelopeSuccessState => EnvelopeOperation.Mapping,
      ValidateEnvelopeErrorState => EnvelopeOperation.Unrecoverable,     // pure validation violated non-throwing contract
      ValidateEnvelopeInvalidErrorState => EnvelopeOperation.Unrecoverable,   // nulls too incomplete for Converting/Confirming
      ValidateEnvelopeInvalidConfirmableErrorState => EnvelopeOperation.Confirming,   // invalid, but skip Converting — offset still confirmable

      // Mapping
      MapEnvelopeSuccessState => EnvelopeOperation.Exit,              // → InboxMessage populated, Status = Mapping
      MapEnvelopeErrorState => EnvelopeOperation.Unrecoverable,       // dev pure mapper violated non-throwing contract
      MapEnvelopeValueErrorState => EnvelopeOperation.Converting,     // bad message → DL path

      // Converting
      ConvertEnvelopeSuccessState => EnvelopeOperation.Exit,          // → DeadLetterEnvelope populated
      ConvertEnvelopeErrorState => EnvelopeOperation.Unrecoverable,   // dev pure FromEnvelope violated contract
      ConvertEnvelopeInvalidState => EnvelopeOperation.Confirming,    // FromEnvelope returned null → skip Redirecting

      // Confirming
      ConfirmEnvelopeSuccessState => EnvelopeOperation.Exit,          // terminal, fully processed
      ConfirmEnvelopeErrorState => EnvelopeOperation.Confirming,      // self-loop, technical

      _ => EnvelopeOperation.Unknown
  };
}