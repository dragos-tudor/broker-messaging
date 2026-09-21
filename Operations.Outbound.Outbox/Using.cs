
global using System;
global using System.Threading;
global using System.Threading.Tasks;
global using Persistence.OutboxMessage;
global using Transport.Envelope;
global using static Foundation.Extensions.ExtensionsFuncs;
global using static Persistence.OutboxMessage.OutboxMessageFuncs;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Pipelines.Outbound")]
[assembly: InternalsVisibleTo("Routing.Outbound")]

namespace Operations.Outbound.Outbox;

public static partial class OutboxFuncs;
