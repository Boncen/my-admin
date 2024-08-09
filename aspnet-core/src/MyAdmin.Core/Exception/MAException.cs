namespace MyAdmin.Core;

public class MAException:Exception
{
    public MAException()
    {
        
    }

    public MAException(string? message)
        : base(message)
    {

    }

    public MAException(string? message, Exception? innerException)
        : base(message, innerException)
    {

    }
}