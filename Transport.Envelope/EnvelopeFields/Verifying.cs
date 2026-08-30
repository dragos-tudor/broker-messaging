
namespace Transport.Envelope;

partial class EnvelopeFuncs
{
  static bool IsValidEnvelopeKey<TKey>(TKey key) => key is not null;
  static bool IsValidEnvelopeType(string type) => !string.IsNullOrWhiteSpace(type);
  static bool IsValidEnvelopeMetadata<TMetadata>(TMetadata metadata) => metadata is not null;
  internal static bool IsValidEnvelopeConfirmation<TConfirmation>(TConfirmation confirmation) => confirmation is not null;
  static bool IsValidEnvelopeValue<TValue>(TValue value) => value is not null;
}