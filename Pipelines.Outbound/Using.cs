global using System;
global using System.Threading;
global using System.Threading.Tasks;
global using Persistence.OutboxMessage;
global using Transport.Envelope;
global using Pipelines.Outbound;
global using static Operations.Outbound.Envelope.EnvelopeFuncs;
global using static Operations.Outbound.Outbox.OutboxFuncs;
global using static Pipelines.Outbound.OutboundFuncs;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Routing.Outbound")]

namespace Pipelines.Outbound;

public static partial class OutboundFuncs;
