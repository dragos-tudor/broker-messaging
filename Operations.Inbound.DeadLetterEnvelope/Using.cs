
global using System;
global using System.Collections.Generic;
global using System.Threading;
global using System.Threading.Tasks;
global using Persistence.DeadLetterMessage;
global using Transport.DeadLetterEnvelope;
global using static Foundation.Extensions.ExtensionsFuncs;
global using static Operations.Inbound.DeadLetterEnvelope.DeadLetterEnvelopeFuncs;
global using static Operations.Inbound.DeadLetterEnvelope.DeadLetterEnvelopeStates;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Pipelines.Inbound")]
[assembly: InternalsVisibleTo("Routing.Inbound")]

namespace Operations.Inbound.DeadLetterEnvelope;

public static partial class DeadLetterEnvelopeFuncs;
