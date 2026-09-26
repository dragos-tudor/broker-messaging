
global using System;
global using System.ComponentModel.DataAnnotations;
global using System.Threading;
global using System.Threading.Tasks;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Operations.Inbound.DeadLetter")]

namespace Persistence.DeadLetterMessage;

public static partial class DeadLetterMessageFuncs;