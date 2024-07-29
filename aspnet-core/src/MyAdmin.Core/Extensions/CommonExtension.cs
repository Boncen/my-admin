using System.Text;

namespace MyAdmin.Core.Extensions;

public static class CommonExtension
{
    public static string FullMessage(this Exception exception)
    {
        if (exception == null)
        {
            return string.Empty;
        }
        string getInfoFromException(Exception ex){
            return ex.Message + Environment.NewLine + ex.Source + Environment.NewLine + ex.StackTrace;
        }
        StringBuilder sb = new StringBuilder();
        sb.Append(getInfoFromException(exception));

        var innerException = exception.InnerException;
        while (innerException != null)
        {
            sb.Append("InnerExceptions:");
            sb.Append(getInfoFromException(innerException));
            innerException = innerException.InnerException;
        }

        return sb.ToString();
    }

    public static string ToCommonString(this DateTime dateTime){
        return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
    }
}
