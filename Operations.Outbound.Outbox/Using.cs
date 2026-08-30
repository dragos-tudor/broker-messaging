
global using System;
global using System.Linq;
global using System.Collections.Generic;
global using System.Threading;
global using System.Threading.Tasks;
global using Persistence.OutboxMessage;
global using Transport.Envelope;
global using static Persistence.OutboxMessage.OutboxMessageFuncs;
using System.Runtime.CompilerServices;

[assembly:InternalsVisibleTo("Pipelines.Outbound")]

namespace Operations.Outbound.Outbox;

public static partial class OutboxFuncs;