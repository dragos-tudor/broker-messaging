global using System;
global using System.Threading;
global using System.Threading.Tasks;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Operations.Inbound.Envelope")]
[assembly: InternalsVisibleTo("Operations.Inbound.Inbox")]
[assembly: InternalsVisibleTo("Operations.Inbound.DeadLetter")]
[assembly: InternalsVisibleTo("Operations.Inbound.DeadLetterEnvelope")]
[assembly: InternalsVisibleTo("Operations.Outbound.Envelope")]
[assembly: InternalsVisibleTo("Operations.Outbound.Outbox")]

namespace Foundation.Extensions;

public static partial class ExtensionsFuncs;