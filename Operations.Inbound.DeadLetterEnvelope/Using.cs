global using System;
global using System.Threading;
global using System.Threading.Tasks;
global using Persistence.DeadLetterMessage;
global using Transport.DeadLetterEnvelope;
global using static Foundation.Extensions.ExtensionsFuncs;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Pipelines.Inbound")]

namespace Operations.Inbound.DeadLetterEnvelope;

public static partial class DeadLetterEnvelopeFuncs;
