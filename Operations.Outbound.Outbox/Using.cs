
global using System;
global using System.Threading;
global using System.Threading.Tasks;
global using Persistence.OutboxMessage;
global using Transport.Envelope;
global using Foundation.Extensions;
global using static Foundation.Extensions.ExtensionsFuncs;
global using static Persistence.OutboxMessage.OutboxMessageFuncs;
global using static Transport.Envelope.EnvelopeFuncs;
global using static Operations.Outbound.Outbox.OutboxFuncs;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Pipelines.Outbound")]
[assembly: InternalsVisibleTo("Routing.Outbound")]

namespace Operations.Outbound.Outbox;

public static partial class OutboxFuncs;
