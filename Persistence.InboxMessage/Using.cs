
global using System;
global using System.Collections.Generic;
global using System.ComponentModel.DataAnnotations;
global using System.Threading;
global using System.Threading.Tasks;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Operations.Inbound.Inbox")]
[assembly: InternalsVisibleTo("Operations.Inbound.Envelope")]

namespace Persistence.InboxMessage;

public static partial class InboxMessageFuncs;