global using System;
global using System.Threading;
global using System.Threading.Tasks;
global using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Routing.Inbound")]
[assembly: InternalsVisibleTo("Routing.Outbound")]

namespace Reliability.Resiliency;

public static partial class ResiliencyFuncs;