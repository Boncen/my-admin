namespace MyAdmin.Core.Exception;

public class MAException:System.Exception
{
    public MAException()
    {
        
    }

    public MAException(string? message)
        : base(message)
    {

    }

    public MAException(string? message, System.Exception? innerException)
        : base(message, innerException)
    {

    }
}