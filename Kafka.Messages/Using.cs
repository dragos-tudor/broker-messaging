
global using System;
global using System.Globalization;
global using System.Text;
global using Confluent.Kafka;
global using static Kafka.Messages.MessagesFuncs;
using System.Runtime.CompilerServices;

[assembly:InternalsVisibleTo("Kafka.Services")]

namespace Kafka.Messages;

public static partial class MessagesFuncs;