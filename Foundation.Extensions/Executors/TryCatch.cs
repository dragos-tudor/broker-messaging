
namespace Foundation.Extensions;

partial class ExtensionsFuncs
{
  internal static ValueTask<(TData, TState, Exception?)> TryCatch<TServices, TData, TState>(
    TServices services,
    TData data,
    Func<TServices, TData, (TData, TState, Exception?)> onSuccess,
    Func<TData, Exception, (TData, TState, Exception?)> onError)
  {
    try
    { return new (onSuccess(services, data)); }
    catch (Exception exception)
    { return new (onError(data, exception)); }
  }

  internal static async ValueTask<(TData, TState, Exception?)> TryCatch<TServices, TData, TState>(
    TServices services,
    TData data,
    Func<TServices, TData, CancellationToken, ValueTask<(TData, TState, Exception?)>> onSuccess,
    Func<TData, Exception, (TData, TState, Exception?)> onError,
    CancellationToken ct = default)
  {
    try
    { return await onSuccess(services, data, ct); }
    catch (OperationCanceledException oce)
    { return (data, default(TState)!, oce); }
    catch (Exception exception)
    { return onError(data, exception); }
  }
}