
global using System;
global using System.Collections.Generic;
global using System.Diagnostics;
global using System.Diagnostics.Metrics;
global using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Router.Inbound")]
[assembly: InternalsVisibleTo("Router.Outbound")]

namespace Observability.Instrumentation;

public static partial class InstrumentationFuncs;