
global using System;
global using System.Threading;
global using System.Threading.Tasks;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Operations.Inbound.Inbox")]
[assembly: InternalsVisibleTo("Operations.Inbound.DeadLetterEnvelope")]

namespace Persistence.RetryPlan;

public static partial class RetryPlanFuncs;
