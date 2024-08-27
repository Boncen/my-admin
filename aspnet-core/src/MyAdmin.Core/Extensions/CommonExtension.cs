using System.ComponentModel;
using System.Reflection;
using System.Text;


namespace MyAdmin.Core.Extensions;

public static class CommonExtension
{
    public static string FullMessage(this System.Exception exception)
    {
        if (exception == null)
        {
            return string.Empty;
        }
        string getInfoFromException(System.Exception ex){
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

    #region Datetime

    public static string ToCommonString(this DateTime dateTime){
        return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
    }

    public static bool IsDefaultValue(this DateOnly date)
    {
        return date.ToString("yyyy-MM-dd").Equals("0001-01-01");
    }
    
    public static bool IsDefaultValue(this DateTime dateTime)
    {
        return dateTime.ToString("yyyy-MM-dd HH:mm:ss").Equals("0001-01-01 00:00:00");
    }
    
    #endregion


    public static IDictionary<string, Object> ToDictionary(this Object obj, bool sort = false)
    {
        IDictionary<string, Object> dic;
        if (sort)
        {
            dic = new SortedDictionary<string, Object>();
        }
        else
        {
            dic = new Dictionary<string, object>();
        }
        var type = obj.GetType();
        var properties = type.GetProperties();
        foreach (var prop in properties)
        {
            var name = prop.Name;
            var value = prop.GetValue(obj);
            dic.Add(name, value);
        }
        return dic;
    }
    /// <summary>
    /// a=1&b=2&c=3 etc.
    /// </summary>
    /// <param name="dic"></param>
    /// <param name="ignoreNull"></param>
    /// <returns></returns>
    public static string ToUrlString(this IDictionary<string, Object> dic,char seperator = '&', bool ignoreNull = true)
    {
        List<string> strings = new List<string>();
        foreach (var key in dic.Keys)
        {
            var val = dic[key];
            if (val == null && ignoreNull)
            {
                continue;
            }
            strings.Add(key + "=" + val?.ToString());
        }
        return String.Join(seperator, strings);
    }

    #region reflection

    public static string GetDescription(this PropertyInfo propertyInfo)
    {
        var descriptionAttr = propertyInfo.GetCustomAttribute<DescriptionAttribute>();
        if (descriptionAttr!= null)
        {
            return descriptionAttr.Description;
        }
        return String.Empty;
    }

    #endregion
}
