
global using System;
global using System.Threading;
global using System.Threading.Tasks;
global using Persistence.InboxMessage;
global using Persistence.DeadLetterMessage;
global using static Persistence.InboxMessage.InboxMessageFuncs;
global using static Persistence.DeadLetterMessage.DeadLetterMessageFuncs;
global using static Foundation.Extensions.ExtensionsFuncs;
global using static Operations.Inbound.Inbox.InboxStates;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Pipelines.Inbound")]
[assembly: InternalsVisibleTo("Routing.Inbound")]

namespace Operations.Inbound.Inbox;

public static partial class InboxFuncs;
