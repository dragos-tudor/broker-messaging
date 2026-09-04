
namespace Operations.Inbound.Envelope;

partial class EnvelopeFuncs
{
  internal static ValueTask<(TData, string, Exception?)> ValidateEnvelope<TServices, TData, TKey, TValue, TMetadata, TConfirmation>(
    TServices services,
    TData data,
    CancellationToken ct = default)
  where TServices : IValidatingServices<TKey, TValue, TMetadata, TConfirmation>
  where TData : IValidatingData<TKey, TValue, TMetadata, TConfirmation>
  {
    try
    {
      var envelope = RequireEnvelope(data.Envelope);
      var valErrors = Transport.Envelope.EnvelopeFuncs.ValidateEnvelope(envelope);

      if (valErrors.Any())
        return IsValidEnvelopeConfirmation(envelope)?
          new ((data, ValidatingInvalidConfirmableError, CreateValidationException(valErrors))):
          new ((data, ValidatingInvalidError, CreateValidationException(valErrors)));

      data.Envelope = envelope;
      return new ((data, ValidatingSuccess, null));
    }
    catch (Exception exception)
    {
      data.PipelineError = exception.Message;
      return new ((data, ValidatingError, exception));
    }
  }
}
