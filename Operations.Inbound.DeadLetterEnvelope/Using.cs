
global using System;
global using System.Threading;
global using System.Threading.Tasks;
global using Persistence.DeadLetterMessage;
global using Persistence.RetryMessage;
global using Transport.DeadLetterEnvelope;
global using static Persistence.DeadLetterMessage.DeadLetterMessageFuncs;
global using static Persistence.RetryMessage.RetryMessageFuncs;
global using static Operations.Inbound.DeadLetterEnvelope.DeadLetterEnvelopeFuncs;
global using static Operations.Inbound.DeadLetterEnvelope.DeadLetterEnvelopeStates;
global using static Transport.DeadLetterEnvelope.DeadLetterEnvelopeFuncs;
using System.Runtime.CompilerServices;

[assembly:InternalsVisibleTo("Pipelines.Inbound")]

namespace Operations.Inbound.DeadLetterEnvelope;

public static partial class DeadLetterEnvelopeFuncs;