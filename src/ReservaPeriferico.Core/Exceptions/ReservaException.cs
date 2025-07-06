namespace ReservaPeriferico.Core.Exceptions;

public class ReservaException : Exception
{
    public ReservaException(string message) : base(message)
    {
    }
    
    public ReservaException(string message, Exception innerException) : base(message, innerException)
    {
    }
} 