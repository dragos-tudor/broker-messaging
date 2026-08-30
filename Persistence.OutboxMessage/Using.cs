
global using System;
global using System.Collections.Generic;
global using System.ComponentModel.DataAnnotations;
global using System.Threading;
global using System.Threading.Tasks;
global using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Operations.Outbound.Envelope")]
[assembly: InternalsVisibleTo("Operations.Outbound.Outbox")]

namespace Persistence.OutboxMessage;

public static partial class OutboxMessageFuncs;