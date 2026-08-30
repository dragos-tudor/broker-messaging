
global using System;
global using System.Diagnostics;
global using System.Diagnostics.Metrics;
global using System.Threading;
global using System.Threading.Tasks;
global using Microsoft.Extensions.Logging;
global using Confluent.Kafka;
global using Kafka.Messages;
global using static Kafka.Messages.MessagesFuncs;
using System.Runtime.CompilerServices;

[assembly:InternalsVisibleTo("Kafka.Services")]

namespace Kafka.Messages;

public static partial class MessagesFuncs;