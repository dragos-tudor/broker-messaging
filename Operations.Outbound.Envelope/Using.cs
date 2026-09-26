
global using System;
global using System.Threading;
global using System.Threading.Tasks;
global using Persistence.OutboxMessage;
global using Transport.Envelope;
global using static Foundation.Extensions.ExtensionsFuncs;
global using static Persistence.OutboxMessage.OutboxMessageFuncs;
global using static Transport.Envelope.EnvelopeFuncs;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Pipelines.Outbound")]
[assembly: InternalsVisibleTo("Routing.Outbound")]

namespace Operations.Outbound.Envelope;

public static partial class EnvelopeFuncs;
