
namespace Foundation.Extensions;

partial class ExtensionsFuncs
{
  internal static TOut Pipe<TIn, TOut>(this TIn input, Func<TIn, TOut> func) =>
    func(input);
}