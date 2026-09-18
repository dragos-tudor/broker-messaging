
namespace Foundation.Extensions;

partial class ExtensionsFuncs
{
  internal static (TData, TState, Exception?) TryCatch<TServices, TData, TState>(
    TServices services,
    TData data,
    Func<TServices, TData, (TData, TState, Exception?)> onSuccess,
    Func<TData, Exception, (TData, TState, Exception?)> onError)
  {
    try
    { return onSuccess(services, data); }
    catch (Exception exception)
    { return onError(data, exception); }
  }

  internal static async Task<(TData, TState, Exception?)> TryCatch<TServices, TData, TState>(
    TServices services,
    TData data,
    Func<TServices, TData, CancellationToken, Task<(TData, TState, Exception?)>> onSuccess,
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