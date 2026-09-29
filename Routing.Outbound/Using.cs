global using System;
global using System.Collections.Generic;
global using System.Threading;
global using System.Threading.Tasks;
global using Foundation.Extensions;
global using Pipelines.Outbound;
global using Reliability.Resiliency;
global using Observability.Instrumentation;
global using Operations.Outbound.Envelope;
global using static Pipelines.Outbound.OutboundFuncs;
global using static Persistence.OutboxMessage.OutboxMessageFuncs;
global using static Operations.Outbound.Outbox.OutboxFuncs;
global using static Routing.Outbound.OutboundFuncs;

namespace Routing.Outbound;

public static partial class OutboundFuncs;
