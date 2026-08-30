global using System;
global using System.Threading;
global using System.Threading.Tasks;
global using static Operations.Inbound.Envelope.EnvelopeFuncs;
global using static Operations.Inbound.DeadLetter.DeadLetterFuncs;
global using static Operations.Inbound.DeadLetterEnvelope.DeadLetterEnvelopeFuncs;
global using static Operations.Inbound.Inbox.InboxFuncs;
using System.Runtime.CompilerServices;

namespace Pipelines.Inbound;

public static partial class InboundFuncs;