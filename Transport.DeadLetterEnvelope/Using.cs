
global using System;
global using System.Threading;
global using System.Threading.Tasks;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Operations.Inbound.DeadLetterEnvelope")]

namespace Transport.DeadLetterEnvelope;

public static partial class DeadLetterEnvelopeFuncs;