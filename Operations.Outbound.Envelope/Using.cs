
global using System;
global using System.Threading;
global using System.Threading.Tasks;
global using Persistence.OutboxMessage;
global using Transport.Envelope;
global using static Transport.Envelope.EnvelopeFuncs;
global using static Persistence.OutboxMessage.OutboxMessageFuncs;
global using static Operations.Outbound.Envelope.EnvelopeFuncs;
global using static Operations.Outbound.Envelope.EnvelopeStates;
using System.Runtime.CompilerServices;

[assembly:InternalsVisibleTo("Pipelines.Outbound")]

namespace Operations.Outbound.Envelope;

public static partial class EnvelopeFuncs;