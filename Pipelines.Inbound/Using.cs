global using System;
global using System.Threading;
global using System.Threading.Tasks;
global using Persistence.DeadLetterMessage;
global using Persistence.InboxMessage;
global using Persistence.RetryPlan;
global using Transport.DeadLetterEnvelope;
global using Transport.Envelope;
global using static Operations.Inbound.Envelope.EnvelopeFuncs;
global using static Operations.Inbound.DeadLetter.DeadLetterFuncs;
global using static Operations.Inbound.DeadLetterEnvelope.DeadLetterEnvelopeFuncs;
global using static Operations.Inbound.Inbox.InboxFuncs;
global using static Operations.Inbound.Envelope.EnvelopeStates;
global using static Operations.Inbound.Inbox.InboxStates;
global using static Operations.Inbound.DeadLetter.DeadLetterStates;
global using static Operations.Inbound.DeadLetterEnvelope.DeadLetterEnvelopeStates;
global using static Pipelines.Inbound.InboundFuncs;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Routing.Inbound")]

namespace Pipelines.Inbound;

public static partial class InboundFuncs;
