using System.ComponentModel;

namespace MyAdmin.Core.Utilities;

public class EnumHelper
{
    public static string GetEnumDesctiption(Enum enumItem)
    {
        var fieldInfo = enumItem.GetType().GetField(enumItem.ToString());
        if (fieldInfo != null)
        {
            var attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(
                typeof(DescriptionAttribute), false);
            if (attributes.Length > 0)
            {
                return attributes[0].Description;
            }
        }
        return enumItem.ToString();
    }
}