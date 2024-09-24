
using System.Reflection;
using MyAdmin.Core.Framework.Attribute;

namespace MyAdmin.Core.Model;

public static class BuiltInModelExtension
{
    public static bool IsBuiltIn(this Entity.Entity entity)
    {
        var type = entity.GetType();
        return type.GetCustomAttribute<BuiltInAttribute>() != null;
    }
}