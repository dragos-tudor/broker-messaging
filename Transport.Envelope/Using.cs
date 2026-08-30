
global using System;
global using System.Threading.Tasks;
global using System.Collections.Generic;
global using System.Threading;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Operations.Inbound.Envelope")]
[assembly: InternalsVisibleTo("Operations.Outbound.Envelope")]

namespace Transport.Envelope;

public static partial class EnvelopeFuncs;