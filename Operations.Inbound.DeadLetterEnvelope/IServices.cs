
namespace Operations.Inbound.DeadLetterEnvelope;

public interface IInstrumentationService { void InstrumentException(Exception exception); }