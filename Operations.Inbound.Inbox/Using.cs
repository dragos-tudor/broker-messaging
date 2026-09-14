
global using System;
global using System.Threading;
global using System.Threading.Tasks;
global using Persistence.InboxMessage;
global using Persistence.DeadLetterMessage;
global using static Persistence.InboxMessage.InboxMessageFuncs;
global using static Persistence.DeadLetterMessage.DeadLetterMessageFuncs;
global using static Foundation.Extensions.ExtensionsFuncs;
global using static Operations.Inbound.Inbox.AbandoningStates;
global using static Operations.Inbound.Inbox.ClosingStates;
global using static Operations.Inbound.Inbox.ConvertingStates;
global using static Operations.Inbound.Inbox.DeadLetteringStates;
global using static Operations.Inbound.Inbox.HandlingStates;
global using static Operations.Inbound.Inbox.InsertingStates;
global using static Operations.Inbound.Inbox.SchedulingStates;
global using static Operations.Inbound.Inbox.TransactingStates;
global using static Operations.Inbound.Inbox.ValidatingStates;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Pipelines.Inbound")]

namespace Operations.Inbound.Inbox;

public static partial class InboxFuncs;
