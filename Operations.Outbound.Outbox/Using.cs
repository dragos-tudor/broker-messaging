
global using System;
global using System.Threading;
global using System.Threading.Tasks;
global using Persistence.OutboxMessage;
global using Transport.Envelope;
global using static Foundation.Extensions.ExtensionsFuncs;
global using static Persistence.OutboxMessage.OutboxMessageFuncs;
global using static Operations.Outbound.Outbox.AbandoningStates;
global using static Operations.Outbound.Outbox.ClosingStates;
global using static Operations.Outbound.Outbox.MappingStates;
global using static Operations.Outbound.Outbox.SchedulingStates;
global using static Operations.Outbound.Outbox.TransactingStates;
global using static Operations.Outbound.Outbox.ValidatingStates;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Pipelines.Outbound")]

namespace Operations.Outbound.Outbox;

public static partial class OutboxFuncs;
