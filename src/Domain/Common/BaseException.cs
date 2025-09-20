namespace GameWeb.Domain.Common;

public class BaseException : Exception
{
    private readonly Exception? _innerException;
    
    public BaseException(string message) : base(message)
    {
    }
    
    public BaseException(string message, Exception innerException) : base(message, innerException)
    {
        _innerException = innerException;
    }

    public override Exception GetBaseException()
    {
        return _innerException?.GetBaseException() ?? this;
    }
}
