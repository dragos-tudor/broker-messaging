
namespace Foundation.Extensions;

partial class ExtensionsFuncs
{
  const string OperationCanceledExceptionState = nameof(OperationCanceledException);

  internal static ValueTask<(TData, string, Exception?)> TryCatch<TServices, TData>(
    TServices services,
    TData data,
    Func<TServices, TData, (TData, string, Exception?)> onSuccess,
    Func<TData, Exception, (TData, string, Exception?)> onError)
  {
    try
    { return new (onSuccess(services, data)); }
    catch (Exception exception)
    { return new (onError(data, exception)); }
  }

  internal static async ValueTask<(TData, string, Exception?)> TryCatch<TServices, TData>(
    TServices services,
    TData data,
    Func<TServices, TData, CancellationToken, ValueTask<(TData, string, Exception?)>> onSuccess,
    Func<TData, Exception, (TData, string, Exception?)> onError,
    CancellationToken ct = default)
  {
    try
    { return await onSuccess(services, data, ct); }
    catch (OperationCanceledException oce)
    { return (data, OperationCanceledExceptionState, oce); }
    catch (Exception exception)
    { return onError(data, exception); }
  }
}