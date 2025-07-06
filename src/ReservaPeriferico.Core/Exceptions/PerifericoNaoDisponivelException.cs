namespace ReservaPeriferico.Core.Exceptions;

public class PerifericoNaoDisponivelException : ReservaException
{
    public PerifericoNaoDisponivelException(string message) : base(message)
    {
    }
    
    public PerifericoNaoDisponivelException(string message, Exception innerException) : base(message, innerException)
    {
    }
} 