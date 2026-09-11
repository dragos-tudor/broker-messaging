
global using System;
global using System.Collections.Generic;
global using System.Threading;
global using System.Threading.Tasks;
global using Persistence.InboxMessage;
global using Transport.Envelope;
global using Pipelines.Inbound;
global using Persistence.DeadLetterMessage;
global using Transport.DeadLetterEnvelope;
global using static Pipelines.Inbound.InboundFuncs;
global using static Routing.Inbound.InboundFuncs;

namespace Routing.Inbound;

public static partial class RoutingFuncs;